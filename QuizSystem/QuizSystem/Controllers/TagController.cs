using DAL.Interfaces;
using QuizSystem.ViewModels.TagViewModel;
using Microsoft.AspNetCore.Mvc;

namespace QuizSystem.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagRepository _tagRepository;
        public TagController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task<IActionResult> Index()
        {
            var tags = await _tagRepository.GetAllTags();

            var tagVM = new TagViewModel();

            var checkboxes = new List<CheckedTagViewModel>();
            foreach (var tag in tags)
            {
                checkboxes.Add(new CheckedTagViewModel() { Description = tag.Description, IsActive = false, TagId = tag.TagId });
            }

            tagVM.CheckedTags = checkboxes;
            return View();
        }

        public async Task SaveTestTags([FromForm] TagViewModel tagViewModel)
        {
            var hello = "jj";
        }
    }
}
