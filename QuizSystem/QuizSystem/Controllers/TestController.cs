using Core.Models;
using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.ViewModels.TestViewModels;

namespace QuizSystem.Controllers
{
    public class TestController : Controller
    {
        private readonly ITestRepository _testRepository;
        public TestController(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int userId)
        {
            //get all user tests
            var tests = await _testRepository.GetUserTests(userId);

            var testVm = new List<IndexTestViewModel>();

            tests.ForEach(
                x => testVm.Add(new IndexTestViewModel()
                {
                    Name = x.Name,
                    Description = x.Description,
                    UserId = x.UserId,
                    Visibility = x.Visibility,
                    DateOfCreation = x.DateOfCreation,
                    TestId = x.TestId,
                }
                )
            );

            ViewBag.UserId = userId;

            return View(testVm);
        }

        [HttpGet]
        public async Task<IActionResult> AllTests()
        {
            var tests = await _testRepository.GetAllTests();

            return View(tests);
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
                TempData["Error"] = "Fail to create test";

                return View(testViewModel);
            }

            var test = new Test()
            {
                Description = testViewModel.Description,
                Name = testViewModel.Name,
                UserId = testViewModel.UserId,
                Visibility = testViewModel.Visibility,
                DateOfCreation = DateTime.Now
            };

            var testId = await _testRepository.AddTest(test);

            return RedirectToAction("Index", "Question", new { testId = testId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int testId, int userId)
        {
            await _testRepository.DeleteTest(testId);

            return RedirectToAction("Index", new { userId = userId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int testId)
        {
            var test = await _testRepository.GetTestById(testId);

            var testVM = new QuestionTestViewModel()
            {
                TestId = testId,
                Description = test.Description,
                Name = test.Name,
                UserId = test.UserId,
                Visibility = test.Visibility,
                DateOfCreation = test.DateOfCreation
            };
            return View(testVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(QuestionTestViewModel testVM)
        {
            var test = new Test()
            {
                TestId = testVM.TestId,
                Description = testVM.Description,
                Name = testVM.Name,
                UserId = testVM.UserId,
                Visibility = testVM.Visibility,
                DateOfCreation=testVM.DateOfCreation
            };

            await _testRepository.UpdateTest(test);

            return RedirectToAction("Index", "Question", new { testId = testVM.TestId });
        }


    }
}
