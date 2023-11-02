using Core.Models;
using DAL.Interfaces;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.ViewModels.QuestionViewModel;
using QuizSystem.ViewModels.TestViewModels;
using BLL.Interfaces;
using QuizSystem.ViewModels;
using QuizSystem.ViewModels.AnswerViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace QuizSystem.Controllers
{
    [Authorize]
    public class QuestionController : Controller
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionService _questionService;
        private readonly IAnswerRepository _answerRepository;
        private readonly IMapper _mapper;
        public QuestionController(IQuestionRepository questionRepository, ITestRepository testRepository, 
            IQuestionService questionService, IAnswerRepository answerRepository,
            IMapper mapper)
        {
            _questionRepository = questionRepository;
            _questionService = questionService;
            _answerRepository = answerRepository;
            _mapper = mapper;
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

            var answers = await _answerRepository.GetQuestionAnswers(questionId);

            var answersVm = answers.Select(a =>
            {
                var answer = _mapper.Map<EditAnswerViewModel>(a);

                return answer;

            }).ToList();

            var questionVM = _mapper.Map<EditQuestionAnswerViewModel>(question);
            questionVM.Answers = answersVm;

            if (questionVM.Type.Equals(QuestionType.Multiple))
            {
                return View("EditAnswerMultiple", questionVM);
            }
            else if (questionVM.Type.Equals(QuestionType.Single))
            {
                return View("EditAnswerSingle", questionVM);
            }
            else
            {
                return View("EditAnswerOpen", questionVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm]EditQuestionAnswerViewModel editQuestionViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Fail to edit question";

                return View(editQuestionViewModel);
            }

            var question = _mapper.Map<Question>(editQuestionViewModel);

            var answerList = editQuestionViewModel.Answers.Select(a =>
            {
                var answer = _mapper.Map<Answer>(a);
                answer.QuestionId = editQuestionViewModel.QuestionId;

                return answer;

            }).ToList();

            var result = await _questionService.EditQuestionAndAnswers(question, answerList);

            if (!result.IsSuccessful)
            {
                TempData["Error"] = result.Message;

                return View(editQuestionViewModel);
            }

            return Json(new { redirectUrl = Url.Action("TestView", "Test", new { testId = editQuestionViewModel.TestId }) });
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestionAndAnswer([FromForm] CreateQuestionViewModel createQuestionViewModel)
        {
            var question = _mapper.Map<Question>(createQuestionViewModel);

            var answerList = createQuestionViewModel.Answers.Select(a =>
            {
                var answer = _mapper.Map<Answer>(a);

                return answer;

            }).ToList();

            var result = await _questionService.AddQuestionWithAnswers(question, answerList);

            if (!result.IsSuccessful)
            {
                TempData["Error"] = result.Message;

                //if (createQuestionViewModel.Type.Equals(QuestionType.Multiple))
                //{
                //    return View("AddAnswerMultiple", createQuestionViewModel);
                //}
                //else if (createQuestionViewModel.Type.Equals(QuestionType.Single))
                //{
                //    return View("AddAnswerSingle", createQuestionViewModel);

                //    //return Json(new { redirectUrl = Url.Action("AddAnswerSingle", new { createQuestionViewModel }) });
                //}
                //else
                //{
                //    return View("AddAnswerOpen", createQuestionViewModel);

                //    //return Json(new { redirectUrl = Url.Action("AddAnswerOpen", new { createQuestionViewModel }) });
                //}

            }

            return Json(new { redirectUrl = Url.Action("TestView", "Test", new { testId = createQuestionViewModel.TestId }) });
        }
    }
}
