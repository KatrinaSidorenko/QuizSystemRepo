using AutoMapper;
using Core.Models;
using DAL.Interfaces;
using DAL.Repository;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.ViewModels.TestViewModels;
using QuizSystem.ViewModels.UserViewModels;

namespace QuizSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var allUsers = await _userRepository.GetAllUsers();
            return View(allUsers);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int userId)
        {
            await _userRepository.DeleteUser(userId);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int userId)
        {
            var user = await _userRepository.GetUserById(userId);

            var userVm = _mapper.Map<EditUserViewModel>(user);

            return View(userVm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = _mapper.Map<User>(model);          

            await _userRepository.UpdateUser(user);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userVm = new CreateUserViewModel();

            return View(userVm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Fail to create user";

                return View(userViewModel);
            }

            var user = _mapper.Map<User>(userViewModel);

            await _userRepository.AddUser(user);

            return RedirectToAction("Index");
        }
    }
}
