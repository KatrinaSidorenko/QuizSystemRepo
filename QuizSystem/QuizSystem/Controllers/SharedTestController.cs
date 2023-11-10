using AutoMapper;
using BLL.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.ViewModels.SharedTestViewModels;

namespace QuizSystem.Controllers
{
    public class SharedTestController : Controller
    {
        private readonly ITestService _testService;
        private readonly ISharedTestService _sharedTestService;
        private readonly IMapper _mapper;
        public SharedTestController(ITestService testService, ISharedTestService sharedTestService,
            IMapper mapper)
        {
            _testService = testService;
            _mapper = mapper;
            _sharedTestService = sharedTestService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int testId)
        {
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

            var sharedTestResult = await _sharedTestService.GetSharedTestByCode(code);

            if (!sharedTestResult.IsSuccessful)
            {
                TempData["Error"] = sharedTestResult.Message;

                return RedirectToAction("Index", "Home");
            }
            //checks of attempts amount, start and end date

            //rediraction to attempt
            return RedirectToAction("TakeTest", "Attempt", new {testId = sharedTestResult.Data.TestId});
        }
    }
}
