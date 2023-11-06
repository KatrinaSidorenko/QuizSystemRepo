using AutoMapper;
using BLL.Interfaces;
using BLL.Services;
using Core.DTO;
using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.ViewModels.AnswerViewModels;
using QuizSystem.ViewModels.AttemptViewModel;
using QuizSystem.ViewModels.QuestionViewModel;
using QuizSystem.ViewModels.TakeTestViewModels;

namespace QuizSystem.Controllers
{
    public class AttemptController : Controller
    {
        private readonly ITestService _testService;
        private readonly IAttemptService _attemptService;
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;
        private readonly ITestResultService _resultService;
        private readonly IMapper _mapper;
        public AttemptController(IAttemptService attemptService, ITestService testService, IMapper mapper,
            IQuestionService questionService, IAnswerService answerService,
            ITestResultService testResultService)
        {
            _attemptService = attemptService;
            _testService = testService;
            _questionService = questionService;
            _answerService = answerService;
            _resultService = testResultService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> TakeTest(int testId)
        {
            var testResult = await _testService.GetTestById(testId);

            if (!testResult.IsSuccessful)
            {
                TempData["Error"] = testResult.Message;

                return RedirectToAction("Index", "Home");
            }

            var questionsResult = await _questionService.GetTestQuestions(testId);

            var questionsVM = questionsResult.Data.Select(async q =>
            {
                var answers = await _answerService.GetQuestionAnswers(q.QuestionId);

                var answersVm = answers.Data.Select(a =>
                {
                    var answer = _mapper.Map<AnswerTakeTestViewModel>(a);

                    return answer;

                }).ToList();

                var question = _mapper.Map<TakeTestQuestionViewModel>(q);
                question.QuestionAnswers = answersVm;

                return question;

            }).ToList();

            var testVM = _mapper.Map<TakeTestViewModel>(testResult.Data);
            testVM.TakeTestQuestions = Task.WhenAll(questionsVM).Result.ToList();
            testVM.TakedTestUserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value);

            var attempt = new Attempt()
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                TestId = testId,
                UserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value),
                SharedTestId = 0
            };

            var attemptId = await _attemptService.AddAttempt(attempt);

            testVM.AttemptId = attemptId.Data;

            return View(testVM);
        }

        [HttpPost]
        public async Task<IActionResult> TakeTest([FromForm] ResultTestViewModel testVM)
        {
            
            var answersDTO = testVM.Answers
                .Select(a => _mapper.Map<AnswerResultDTO>(a)).ToList();

            var testDTO = _mapper.Map<AttemptResultDTO>(testVM);
            testDTO.Answers = answersDTO;

            var answers = testVM.Answers
                .Select(a => _mapper.Map<Answer>(a)).ToList();

            var saveAnswersResult = await _attemptService.SaveUserGivenAnswers(answers, testVM.AttemptId);

            var attemptResult = await _attemptService.SaveAttemptData(testDTO);

           
            return Json(new { redirectUrl = Url.Action("Result", "Attempt", new { attemptId = attemptResult.Data.AttemptId }) });
        }

        [HttpGet]
        public async Task<IActionResult> Result(int attemptId)
        {
            var attempt = await _attemptService.GetAttemptById(attemptId);
            var attemptVm = _mapper.Map<AttemptViewModel>(attempt.Data);

            return View(attemptVm);
        }

        [HttpGet]
        public async Task<IActionResult> AttemptResult(int attemptId)
        {
            var attemptResult = await _attemptService.GetAttemptById(attemptId);

            if (!attemptResult.IsSuccessful)
            {
                TempData["Error"] = "Fail to get attempt";

                return View("Result", attemptId);
            }

            var attemptVm = new AttemptResultViewModel();
            var testResult = await _testService.GetTestById(attemptResult.Data.TestId);

            attemptVm.TestId = testResult.Data.TestId;
            attemptVm.Name = testResult.Data.Name;

            var questionsResult = await _questionService.GetTestQuestions(testResult.Data.TestId);

            var questionsVM = questionsResult.Data.Select(async q =>
            {
                var answers = await _answerService.GetQuestionAnswers(q.QuestionId);
                var testResult = await _resultService.GetTestResult(attemptId, q.QuestionId);

                var answersVm = answers.Data.Select( a =>
                {
                    var attemptAnswer = new AttemptAnswerViewModel()
                    {
                        AnswerId = a.AnswerId,
                        IsRight = a.IsRight,
                        Value = a.Value
                    };

                    
                    if (q.Type.Equals(QuestionType.Open))
                    {
                        attemptAnswer.ChoosenByUser = testResult.Data.EnteredValue.ToLower().Equals(a.Value)? true : false;
                        attemptAnswer.ValueByUser = testResult.Data.EnteredValue;
                    }
                    else
                    {
                        attemptAnswer.ChoosenByUser = a.AnswerId == testResult.Data.AnswerId ? true : false;
                    }

                    
                    return attemptAnswer;

                }).ToList();

                var question = new AttemptQuestionViewModel()
                {
                    QuestionId = q.QuestionId,
                    Description = q.Description,
                    Point = q.Point,
                    Type = q.Type,
                    Answers = answersVm
                };

                return question;

            }).ToList();

            attemptVm.Questions = Task.WhenAll(questionsVM).Result.ToList();

            return View(attemptVm);
        }
    }
}
