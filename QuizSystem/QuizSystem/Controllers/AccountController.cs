using AutoMapper;
using BLL.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using QuizSystem.ViewModels.UserViewModels;
using System.Security.Claims;

namespace QuizSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public AccountController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var loginViewModel = new LoginViewModel();

            return View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid user data input";

                return View(loginViewModel);
            }

            var userExistResult = await _userService.IsUserExist(loginViewModel.Email, loginViewModel.Password);

            if (!userExistResult.IsSuccessful)
            {
                TempData["Error"] = userExistResult.Message;

                return View(loginViewModel);
            }

            await Autthenticate(loginViewModel.Email, userExistResult.Data.UserId);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var registerUser = new CreateUserViewModel();

            return View(registerUser);
        }

        [HttpPost]
        public async Task<IActionResult> Register(CreateUserViewModel newUser)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid user data input";

                return View(newUser);
            }

            var emailCheckResult = await _userService.IsTheEmailAvailable(newUser.Email);

            if (!emailCheckResult.IsSuccessful)
            {
                TempData["Error"] = emailCheckResult.Message;

                return View(newUser);
            }

            var user = _mapper.Map<User>(newUser);

            var registerResult = await _userService.AddUser(user);

            if (!registerResult.IsSuccessful)
            {
                TempData["Error"] = registerResult.Message;

                return View(newUser);
            }

            await Autthenticate(newUser.Email, registerResult.Data);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

        private async Task Autthenticate(string email, int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, email),
                new Claim("id", userId.ToString()),
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
