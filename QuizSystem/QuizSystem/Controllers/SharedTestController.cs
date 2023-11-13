using AutoMapper;
using BLL.Interfaces;
using BLL.Services;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
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

            var sharedTest = _mapper.Map<SharedTest>(createShareTest);
           
            sharedTest.AttemptDuration = new DateTime(2023, 01, 01).AddMinutes(createShareTest.AttemptDuration);

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
            //existence of code

            var sharedTestResult = await _sharedTestService.GetSharedTestByCode(code, Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value));

            if (!sharedTestResult.IsSuccessful)
            {
                TempData["Error"] = sharedTestResult.Message;

                return RedirectToAction("Index", "Home");
            }
            //checks of attempts amount, start and end date

            //rediraction to attempt
            return RedirectToAction("TakeTest", "Attempt", new {testId = sharedTestResult.Data.TestId, sharedTestId = sharedTestResult.Data.SharedTestId});
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value);

            var userIdResult = await _userService.IsUserExist(userId);

            if (!userIdResult.IsSuccessful)
            {
                TempData["Error"] = userIdResult.Message;

                return RedirectToAction("Index", "Home");
            }

            var sharedTestsResult = await _sharedTestService.GetUserSharedTests(userId);

            if (!sharedTestsResult.IsSuccessful)
            {
                TempData["Error"] = sharedTestsResult.Message;

                return RedirectToAction("Index", "Home");
            }

            var sharedVms = sharedTestsResult.Data.Select(t => _mapper.Map<IndexSharedTestViewModel>(t)).ToList();


            return View(sharedVms);
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
    }
}
