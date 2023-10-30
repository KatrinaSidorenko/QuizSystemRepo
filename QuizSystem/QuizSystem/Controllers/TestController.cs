using AutoMapper;
using BLL.Interfaces;
using Core.Models;
using DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.ViewModels.TestViewModels;

namespace QuizSystem.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        private readonly ITestService _testService;
        private readonly IMapper _mapper;
        public TestController(ITestService testRepository, IMapper mapper)
        {
            _testService = testRepository;
            _mapper = mapper;
        }

        [HttpGet]

        public async Task<IActionResult> Index(int id)
        {
            var testsResult = await _testService.GetUserTests(id);

            if (!testsResult.IsSuccessful)
            {
                TempData["Error"] = testsResult.Message;

                return View(testsResult.Data);
            }

            var testVm = new List<IndexTestViewModel>();

            testsResult.Data.ForEach(
                t => testVm.Add(_mapper.Map<IndexTestViewModel>(t))
            );

            ViewBag.UserId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "id").Value);

            return View(testVm);
        }

        [HttpGet]
        public async Task<IActionResult> AllTests()
        {
            var testsResult = await _testService.GetAllPublicTests();

            if (!testsResult.IsSuccessful)
            {
                TempData["Error"] = testsResult.Message;
            }

            return View(testsResult.Data);
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

            return RedirectToAction("Index", "Question", new { testId = testId.Data });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int testId, int userId)
        {
            var deleteResult = await _testService.DeleteTest(testId);

            if (!deleteResult.IsSuccessful)
            {
                TempData["Error"] = deleteResult.Message;
            }

            return RedirectToAction("Index", new { userId = userId });
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

            return RedirectToAction("Index", "Question", new { testId = testVM.TestId });
        }


    }
}
