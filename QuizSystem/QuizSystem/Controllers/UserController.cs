using AutoMapper;
using BLL.Interfaces;
using Core.Models;
using DAL.Interfaces;
using DAL.Repository;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.ViewModels.UserViewModels;

namespace QuizSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var allUsers = await _userService.GetAllUsers();

            return View(allUsers.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int userId)
        {
            var deleteResult = await _userService.DeleteUser(userId);

            if (!deleteResult.IsSuccessful)
            {
                TempData["Error"] = deleteResult.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int userId)
        {
            var userResult = await _userService.GetUserById(userId);

            if (!userResult.IsSuccessful)
            {
                TempData["Error"] = userResult.Message;
            }

            var userVm = _mapper.Map<EditUserViewModel>(userResult.Data);

            return View(userVm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = _mapper.Map<User>(model);          

            await _userService.UpdateUser(user);

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

            await _userService.AddUser(user);

            return RedirectToAction("Index");
        }
    }
}
