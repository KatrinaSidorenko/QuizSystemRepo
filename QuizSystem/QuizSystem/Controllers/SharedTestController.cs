using AutoMapper;
using BLL.Interfaces;
using BLL.Services;
using Core.Enums;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.ViewModels.PaginationTestViewModels;
using QuizSystem.ViewModels.SharedTestViewModels;

namespace QuizSystem.Controllers
{
    public class SharedTestController : Controller
    {
        private readonly ITestService _testService;
        private readonly ISharedTestService _sharedTestService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public SharedTestController(ITestService testService, ISharedTestService sharedTestService,
            IMapper mapper, IUserService userService)
        {
            _testService = testService;
            _mapper = mapper;
            _userService = userService;
            _sharedTestService = sharedTestService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int testId)
        {
            var questionsAmountAllow = await _testService.IsInTestQuestions(testId);

            if (!questionsAmountAllow.IsSuccessful || !questionsAmountAllow.Data)
            {
                var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value);
                TempData["Error"] = questionsAmountAllow.Message;

                return RedirectToAction("Index", "Test", new { id = userId });
            }

            var permissionResult = await _sharedTestService.IsTestShared(testId);

            if (!permissionResult.IsSuccessful)
            {
                var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value);
                TempData["Error"] = permissionResult.Message;

                return RedirectToAction("Index", "Test", new {id = userId });
            }

            var testResult = await _testService.GetTestById(testId);

            if (!testResult.IsSuccessful)
            {
                TempData["Error"] = testResult.Message;

                return RedirectToAction("Index", "Home");
            }

            if (testResult.Data.Visibility.Equals(Visibility.Private))
            {
                TempData["Error"] = "The test must be public in order to conduct general testing";

                return RedirectToAction("Index", "Test", new { id = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id").Value)});
            }

            var shareTestVM = new CreateShareTestViewModel { TestId = testId, TestName = testResult.Data.Name};

            return View(shareTestVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateShareTestViewModel createShareTest)
        {
            if (!ModelState.IsValid)
            {
                return View(createShareTest);
            }

            if (createShareTest.AttemptCount <= 0)
            {
                TempData["Error"] = "Attempt count can be 0 or less";

                return View(createShareTest);
            }

            if (createShareTest.EndDate < createShareTest.StartDate)
            {
                TempData["Error"] = "End date can't be earlier than start date";

                return View(createShareTest);
            }

            if (createShareTest.EndDate < DateTime.Now)
            {
                TempData["Error"] = "Test can't be completed at start";

                return View(createShareTest);
            }

            var testMaxMarkResult = await _testService.GetQuestionsAmountAndMaxMark(createShareTest.TestId);
            var passingScore = Convert.ToDouble(createShareTest.PassScore.Replace('.', ','));

            if (passingScore > testMaxMarkResult.Data.Item2)
            {
                TempData["Error"] = $"The passing score must be lower than the maximum score on the test. Maximum score for this test is {testMaxMarkResult.Data.Item2}";

                return View(createShareTest);
            }

            var sharedTest = _mapper.Map<SharedTest>(createShareTest);
           
            sharedTest.AttemptDuration = new DateTime(2023, 01, 01).AddMinutes(createShareTest.AttemptDuration);

            sharedTest.PassingScore = passingScore;

            var addResult = await _sharedTestService.AddSharedTest(sharedTest);

            if (!addResult.IsSuccessful)
            {
                TempData["Error"] = addResult.Message;

                return View(createShareTest);
            }

            return RedirectToAction("Details", "SharedTest", new { sharedTestId = addResult.Data });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int sharedTestId)
        {
            var sharedTestResult = await _sharedTestService.GetSharedTestById(sharedTestId);

            if (!sharedTestResult.IsSuccessful)
            {
                TempData["Error"] = sharedTestResult.Message;

                return RedirectToAction("Index", "Home");
            }

            var testResult = await _testService.GetTestById(sharedTestResult.Data.TestId);

            if (!sharedTestResult.IsSuccessful)
            {
                TempData["Error"] = sharedTestResult.Message;

                return RedirectToAction("Index", "Home");
            }

            var testVM = _mapper.Map<DetailsSharedTestViewModel>(sharedTestResult.Data);
            testVM.TestName = testResult.Data.Name;

            return View(testVM);
        }

        [HttpGet]
        public async Task<IActionResult> GetCode(Guid code)
        {
            var sharedTestResult = await _sharedTestService.GetSharedTestByCode(code, Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value));

            if (!sharedTestResult.IsSuccessful)
            {
                TempData["Error"] = sharedTestResult.Message;

                return RedirectToAction("Index", "Home");
            }

            var agreement = new AgreementSharedTestViewModel()
            {
                TestId = sharedTestResult.Data.TestId,
                SharedTestId = sharedTestResult.Data.SharedTestId,
                PassingScore = sharedTestResult.Data.PassingScore,
                AttemptDuration = sharedTestResult.Data.AttemptDuration,
                Description = sharedTestResult.Data.Description,
                AttemptCount = sharedTestResult.Data.AttemptCount
            };

            return View("Agreement", agreement);
        }

        [HttpGet]
        public async Task<IActionResult> Index(SortingParam sortOrder, SharedTestStatus? filterParam = null, int page = 1, string searchParam = "")
        {
            int pageSize = 3;
            string search = string.IsNullOrEmpty(searchParam) ? "" : searchParam.ToLower();
            
            var testPaginationModel = new SharedTestPaginationModel()
            {
                CurrentPageIndex = page > 0 ? page : 1,
                SearchParam = search,
                SortingParam = sortOrder
            };


            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value);

            var userIdResult = await _userService.IsUserExist(userId);

            if (!userIdResult.IsSuccessful)
            {
                TempData["Error"] = userIdResult.Message;

                return RedirectToAction("Index", "Home");
            }

            var sharedTestsResult = await _sharedTestService.GetUserSharedTests(userId, sortOrder, filterParam, page, pageSize, searchParam);

            if (!sharedTestsResult.IsSuccessful)
            {
                TempData["Error"] = sharedTestsResult.Message;

                return RedirectToAction("Index", "Home");
            }

            var sharedVms = sharedTestsResult.Data.Item1.Select(t => _mapper.Map<IndexSharedTestViewModel>(t)).ToList();
            double pageCount;

            if (!string.IsNullOrEmpty(searchParam))
            {
                pageCount = (double)((decimal)sharedVms.Count() / Convert.ToDecimal(pageSize));
            }
            else
            {
                pageCount = (double)((decimal)sharedTestsResult.Data.Item2 / Convert.ToDecimal(pageSize));
            }

            testPaginationModel.PageCount = (int)Math.Ceiling(pageCount);
            testPaginationModel.PageSize = pageSize;
            testPaginationModel.Tests = sharedVms;

            return View(testPaginationModel);
        }

        [HttpGet]
        public async Task<IActionResult> Finish(int sharedTestId)
        {
            var sharedTestFinish = await _sharedTestService.FinishSharedTest(sharedTestId);

            if (!sharedTestFinish.IsSuccessful)
            {
                TempData["Error"] = sharedTestFinish.Message;

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "SharedTest");
        }

        public async Task<IActionResult> Delete(int sharedTestId)
        {
            var deleteResult = await _sharedTestService.DeleteSharedTest(sharedTestId);

            if (!deleteResult.IsSuccessful)
            {
                TempData["Error"] = deleteResult.Message;

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "SharedTest");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int sharedTestId)
        {
            var sharedTestResult = await _sharedTestService.GetSharedTestById(sharedTestId);

            if (!sharedTestResult.IsSuccessful)
            {
                TempData["Error"] = sharedTestResult.Message;

                return RedirectToAction("Details", "SharedTest", new { sharedTestId = sharedTestId });
            }

            var sharedTestVm = _mapper.Map<EditSharedTestViewModel>(sharedTestResult.Data);

            return View(sharedTestVm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditSharedTestViewModel sharedTestVm)
        {
            if(sharedTestVm.AttemptCount <= 0)
            {
                TempData["Error"] = "Attempt count can be 0 or less";

                return View(sharedTestVm);
            }

            if (sharedTestVm.EndDate < sharedTestVm.StartDate)
            {
                TempData["Error"] = "End date can't be earlier than start date";

                return View(sharedTestVm);
            }

            if (sharedTestVm.EndDate < DateTime.Now)
            {
                TempData["Error"] = "Test can't be completed at start";

                return View(sharedTestVm);
            }

            var sharedTest = _mapper.Map<SharedTest>(sharedTestVm);
            sharedTest.AttemptDuration = new DateTime(2023, 01, 01).AddMinutes(sharedTestVm.NewAttemptDuration);
            sharedTest.PassingScore = Convert.ToDouble(sharedTestVm.PassScore.Replace('.', ','));
            var updateResult = await _sharedTestService.UpdateSharedTest(sharedTest);

            if (!updateResult.IsSuccessful)
            {
                TempData["Error"] = updateResult.Message;

                return View(sharedTestVm);
            }

            return RedirectToAction("Details", "SharedTest", new { sharedTestId = sharedTestVm.SharedTestId });
        }

        [HttpGet]
        public async Task<IActionResult> Statistic(int sharedTestId)
        {
            var sharedTestResult = await _sharedTestService.GetSharedTestStatistic(sharedTestId);
            if (!sharedTestResult.IsSuccessful)
            {
                TempData["Error"] = sharedTestResult.Message;

                return RedirectToAction("Details", "SharedTest", new {sharedTestId = sharedTestId});
            }

            var sharedTestVm = _mapper.Map<StatisticSharedTestViewModel>(sharedTestResult.Data);
            return View(sharedTestVm);
        }
    }
}
