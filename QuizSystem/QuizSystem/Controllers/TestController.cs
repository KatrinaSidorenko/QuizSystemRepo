using AutoMapper;
using BLL.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Interfaces;
using DAL.Repository;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.ViewModels.AnswerViewModels;
using QuizSystem.ViewModels.PaginationTestViewModels;
using QuizSystem.ViewModels.QuestionViewModel;
using QuizSystem.ViewModels.TakeTestViewModels;
using QuizSystem.ViewModels.TestViewModels;

namespace QuizSystem.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        private readonly ITestService _testService;
        private readonly IAnswerRepository _answerRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IAttemptService _attemptService;
        private readonly IMapper _mapper;
        public TestController(ITestService testRepository, IMapper mapper, IQuestionRepository questionRepository, 
            IAnswerRepository answerRepository, IAttemptService attemptService)
        {
            _testService = testRepository;
            _mapper = mapper;
            _answerRepository = answerRepository;
            _questionRepository = questionRepository;
            _attemptService = attemptService;
        }


        [HttpGet]

        public async Task<IActionResult> Index(int id, SortingParam sortOrder, Visibility? filterParam = null, int page = 1, string searchParam = "")
        {
            int pageSize = 6;
            string search = string.IsNullOrEmpty(searchParam) ? "" : searchParam.ToLower();

            var testPaginationModel = new TestPaginationModel()
            {
                CurrentPageIndex = page > 0 ? page : 1,
                SearchParam = search,
                UserId = id,
                SortingParam = sortOrder,
                FilterParam = filterParam
            };
            var testsResult = await _testService.GetUserTests(id, sortOrder, filterParam,
                page, pageSize, search);

            if (!testsResult.IsSuccessful)
            {
                TempData["Error"] = testsResult.Message;

                return View(testPaginationModel);
            }


            var testVm = testsResult.Data.Item1.Select( t => 
                {
                    var test = _mapper.Map<IndexTestViewModel>(t);
                    return test;
                }
            );
            var pageCount = (double)((decimal)testsResult.Data.Item2 / Convert.ToDecimal(pageSize));
            testPaginationModel.Tests = testVm.ToList();
            testPaginationModel.PageCount = (int)Math.Ceiling(pageCount);
            testPaginationModel.PageSize = pageSize;

            ViewBag.UserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id").Value);

            return View(testPaginationModel);
        }


        [HttpGet]
        public async Task<IActionResult> TestView(int testId)
        {
            var test = await _testService.GetTestById(testId);

            if (!test.IsSuccessful)
            {
                TempData["Error"] = test.Message;

                return RedirectToAction("Index", "Home");
            }

            var questionsAmount = await _testService.GetQuestionsAmountAndMaxMark(testId);

            if (!questionsAmount.IsSuccessful)
            {
                TempData["Error"] = questionsAmount.Message;

                return RedirectToAction("Index", "Home");
            }

            var questions = await _questionRepository.GetTestQuestions(testId);

            var questionsVM = questions.Select(async q =>
            {
                var answers = await _answerRepository.GetQuestionAnswers(q.QuestionId);

                var answersVm = answers.Select(a =>
                {
                    var answer = _mapper.Map<AnswerViewModel>(a);

                    return answer;

                }).ToList();

                var question = _mapper.Map<IndexQuestionViewModel>(q);
                question.Answers = answersVm;

                return question;

            }).ToList();
            //get all question for this test
            var testVM = _mapper.Map<MemberTestViewModel>(test.Data);
            testVM.Questions = Task.WhenAll(questionsVM).Result.ToList();
            testVM.AmountOfQuestions = questionsAmount.Data.Item1;
            testVM.TotalMark = questionsAmount.Data.Item2;

            return View(testVM);
        }

        [HttpGet]
        public async Task<IActionResult> MemberView(int testId)
        {
            var test = await _testService.GetTestById(testId);

            if (!test.IsSuccessful)
            {
                TempData["Error"] = test.Message;

                return RedirectToAction("Index", "Home");
            }

            var questionsAmount = await _testService.GetQuestionsAmountAndMaxMark(testId);

            if (!questionsAmount.IsSuccessful)
            {
                TempData["Error"] = questionsAmount.Message;

                return RedirectToAction("Index", "Home");
            }

            var questions = await _questionRepository.GetTestQuestions(testId);

            var questionsVM = questions.Select(async q =>
            {
                var answers = await _answerRepository.GetQuestionAnswers(q.QuestionId);

                var answersVm = answers.Select(a =>
                {
                    var answer = _mapper.Map<AnswerViewModel>(a);

                    return answer;

                }).ToList();

                var question = _mapper.Map<IndexQuestionViewModel>(q);
                question.QuestionId = q.QuestionId;
                question.Answers = answersVm;

                return question;

            }).ToList();

            var testVM = _mapper.Map<MemberTestViewModel>(test.Data);
            testVM.Questions = Task.WhenAll(questionsVM).Result.ToList();
            testVM.AmountOfQuestions = questionsAmount.Data.Item1;
            testVM.TotalMark = questionsAmount.Data.Item2;

            return View(testVM);
        }

        [HttpGet]
        public async Task<IActionResult> AllTests(SortingParam sortOrder, int page = 1, string searchParam = "")
        {
            int pageSize = 6;
            string search = string.IsNullOrEmpty(searchParam) ? "" : searchParam.ToLower();
            
            var testPaginationModel = new TestPaginationModel()
            {
                CurrentPageIndex = page > 0 ? page : 1,
                SearchParam = search,
                SortingParam = sortOrder
            };
 

            var testsResult = await _testService.GetAllPublicTests(pageNumber: page, pageSize: pageSize, search:search, sortingParam: sortOrder);

            if (!testsResult.IsSuccessful)
            {
                TempData["Error"] = testsResult.Message;
            }

            if (testsResult.Data.Item1 is not null)
            {
                var attemptcountResult = await _testService.GetTestAttemptsCount();

                var testVMS = testsResult.Data.Item1.Select(t =>
                {
                    var testVm = _mapper.Map<IndexTestViewModel>(t);

                    if (attemptcountResult.Data.ContainsKey(t.TestId))
                    {
                        testVm.UserTakenTestAmount = attemptcountResult.Data[t.TestId];
                    }

                    return testVm;
                });

                testPaginationModel.Tests = testVMS.ToList();

                double pageCount;

                if (!string.IsNullOrEmpty(searchParam))
                {
                    pageCount = (double)((decimal)testVMS.Count() / Convert.ToDecimal(pageSize));
                }
                else
                {
                    pageCount = (double)((decimal)testsResult.Data.Item2 / Convert.ToDecimal(pageSize));

                }

                testPaginationModel.PageCount = (int)Math.Ceiling(pageCount);
                testPaginationModel.PageSize = pageSize;
            }

            return View(testPaginationModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int userId)
        {
            var testVM = new TestViewModel { UserId = userId };

            return View(testVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TestViewModel testViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid data input";

                return View(testViewModel);
            }

            var test = _mapper.Map<Test>(testViewModel);
            test.DateOfCreation = DateTime.Now;

            var testId = await _testService.AddTest(test);

            if (!testId.IsSuccessful)
            {
                TempData["Error"] = testId.Message;

                return View(testViewModel);
            }

            return RedirectToAction("TestView", "Test", new { testId = testId.Data });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int testId, int userId)
        {
            var deleteResult = await _attemptService.DeleteTestWithAttempts(testId);

            if (!deleteResult.IsSuccessful)
            {
                TempData["Error"] = deleteResult.Message;
            }

            return RedirectToAction("Index", "Test", new { id = userId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int testId)
        {
            var testResult = await _testService.GetTestById(testId);

            if (!testResult.IsSuccessful)
            {
                TempData["Error"] = testResult.Message;

                return RedirectToAction("Index", "Question", new { testId = testId });
            }

            var testVM = _mapper.Map<QuestionTestViewModel>(testResult.Data);

            return View(testVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(QuestionTestViewModel testVM)
        {
            var test = _mapper.Map<Test>(testVM);

            var updateResult = await _testService.UpdateTest(test);

            if (!updateResult.IsSuccessful)
            {
                TempData["Error"] = updateResult.Message;

                return RedirectToAction("Index", "Question", new { testId = testVM.TestId });
            }

            return RedirectToAction("TestView", "Test", new { testId = testVM.TestId });
        }

        [HttpGet]
        public async Task<IActionResult> TestDocumentView(int testId)
        {
            var pathResult = await _testService.GetTestDocumentPath(testId);

            if(!pathResult.IsSuccessful)
            {
                return RedirectToAction("Index", "Home");
            }

            var outputPath = Path.Combine("Output/Tests", pathResult.Data.Item1);

            using (Viewer viewer = new Viewer(pathResult.Data.Item2))
            {
                PdfViewOptions options = new PdfViewOptions(outputPath);
                viewer.View(options);
            }

            var fileStream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            var result = new FileStreamResult(fileStream, "application/pdf");

            return result;
        }
        

    }
}
