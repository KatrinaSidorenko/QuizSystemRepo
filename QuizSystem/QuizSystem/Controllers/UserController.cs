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
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

            var userVm = new EditUserViewModel() 
            { 
                UserId = userId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Password = user.Password
            };

            return View(userVm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = new User()
            {
                UserId = model.UserId,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth.Date,
                Password = model.Password
            };

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

            var user = new User()
            {
                Email = userViewModel.Email,
                FirstName = userViewModel.FirstName,
                LastName = userViewModel.LastName,
                DateOfBirth = userViewModel.DateOfBirth.Date,
                Password = userViewModel.Password
            };

            await _userRepository.AddUser(user);

            return RedirectToAction("Index");
        }
    }
}
