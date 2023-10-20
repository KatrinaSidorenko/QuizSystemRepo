using Core.Models;
using DAL.Interfaces;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.ViewModels.QuestionViewModel;
using QuizSystem.ViewModels.TestViewModels;
using BLL.Interfaces;

namespace QuizSystem.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ITestRepository _testRepository;
        private readonly IQuestionService _questionService;
        public QuestionController(IQuestionRepository questionRepository, ITestRepository testRepository, IQuestionService questionService)
        {
            _questionRepository = questionRepository;
            _testRepository = testRepository;
            _questionService = questionService;

        }

        [HttpGet]
        public async Task<IActionResult> Index(int testId)
        {
            var test = await _testRepository.GetTestById(testId);
            var questions = await _questionRepository.GetTestQuestions(testId);
            //get all question for this test

            var testVM = new QuestionTestViewModel()
            {
                TestId = testId,
                Description = test.Description,
                Name = test.Name,
                UserId = test.UserId,
                Visibility = test.Visibility,
                DateOfCreation = test.DateOfCreation,
                Questions  = questions,
            };

            return View(testVM);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int testId)
        {
            var questionVM = new CreateQuestionViewModel() { TestId = testId };

            return View(questionVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateQuestionViewModel questionViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Fail to create question";

                return View(questionViewModel);
            }

            if (questionViewModel.Type.Equals(QuestionType.Multiple))
            {
                return View("AddAnswerMultiple", questionViewModel);
            }
            else if (questionViewModel.Type.Equals(QuestionType.Single))
            {
                return View("AddAnswerSingle", questionViewModel);
            }
            else
            {
                return View("AddAnswerOpen", questionViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int questionId, int testId)
        {
            await _questionRepository.DeleteQuestion(questionId);

            return RedirectToAction("Index", new {testId = testId});
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int questionId)
        {
            var question = await _questionRepository.GetQuestionById(questionId);

            var questionVM = new EditQuestionViewModel()
            {
                TestId = question.TestId,
                Description = question.Description,
                Type = question.Type,
                Point = question.Point,
                QuestionId = questionId
            };

            return View(questionVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditQuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Fail to edit question";

                return View(model);
            }

            var question = new Question()
            {
                QuestionId = model.QuestionId,
                Description = model.Description,
                Type = model.Type,
                Point = model.Point,
                TestId = model.TestId
            };

            await _questionRepository.UpdateQuestion(question);

            return RedirectToAction("Index", new {testId =  model.TestId});
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestionAndAnswer([FromForm] CreateQuestionViewModel createQuestionViewModel)
        {
             var question = new Question()
            {
                TestId = createQuestionViewModel.TestId,
                Description = createQuestionViewModel.Description,
                Type = createQuestionViewModel.Type,
                Point = createQuestionViewModel.Point
            };

            var answerList = createQuestionViewModel.Answers.Select(a =>
            {
                var answer = new Answer()
                {
                    Value = a.Value,
                    IsRight = a.IsRight
                };

                return answer;
            }).ToList();

            var result = await _questionService.AddQuestionWithAnswers(question, answerList);

            if (!result.IsSuccessful)
            {
                TempData["Error"] = result.Message;

                if (createQuestionViewModel.Type.Equals(QuestionType.Multiple))
                {
                    return View("AddAnswerMultiple", createQuestionViewModel);
                }
                else if (createQuestionViewModel.Type.Equals(QuestionType.Single))
                {
                    return View("AddAnswerSingle", createQuestionViewModel);
                }
                else
                {
                    return View("AddAnswerOpen", createQuestionViewModel);
                }

            }

            return Json(new { redirectUrl = Url.Action("Index", new { testId = createQuestionViewModel.TestId }) });
        }
    }
}
