using AutoMapper;
using Core.Models;
using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.ViewModels.TestViewModels;

namespace QuizSystem.Controllers
{
    public class TestController : Controller
    {
        private readonly ITestRepository _testRepository;
        private readonly IMapper _mapper;
        public TestController(ITestRepository testRepository, IMapper mapper)
        {
            _testRepository = testRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int userId)
        {
            //get all user tests
            var tests = await _testRepository.GetUserTests(userId);

            var testVm = new List<IndexTestViewModel>();

            tests.ForEach(
                t => testVm.Add(_mapper.Map<IndexTestViewModel>(t))
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

            var test = _mapper.Map<Test>(testViewModel);
            test.DateOfCreation = DateTime.Now;

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

            var testVM = _mapper.Map<QuestionTestViewModel>(test);

            return View(testVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(QuestionTestViewModel testVM)
        {
            var test = _mapper.Map<Test>(testVM);

            await _testRepository.UpdateTest(test);

            return RedirectToAction("Index", "Question", new { testId = testVM.TestId });
        }


    }
}
