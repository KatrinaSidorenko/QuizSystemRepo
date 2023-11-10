using Microsoft.AspNetCore.Mvc;
using QuizSystem.ViewModels.SharedTestViewModels;

namespace QuizSystem.Controllers
{
    public class SharedTestController : Controller
    {
        public SharedTestController()
        {
            
        }

        public async Task<IActionResult> Create(int testId)
        {
            var shareTestVM = new CreateShareTestViewModel { TestId = testId };

            return View(shareTestVM);
        }
    }
}
