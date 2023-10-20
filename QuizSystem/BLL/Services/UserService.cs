using BLL.Interfaces;
using Core.Models;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private  IHttpContextAccessor _contextAccessor;
        public UserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _contextAccessor = httpContextAccessor;
        }
        public async Task<Result<List<User>>> GetAllUsers()
        {
            try
            {
               //await  _contextAccessor.HttpContext.Response.WriteAsync("Hello");
                var users = await _userRepository.GetAllUsers();

                return new Result<List<User>>(true, users);
            }
            catch (Exception ex)
            {
                return new Result<List<User>>(false, $"Fail to get all users");
            }
        }
    }
}
