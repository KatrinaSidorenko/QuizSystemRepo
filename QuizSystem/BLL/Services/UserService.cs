using BLL.Interfaces;
using Core.Models;
using DAL.Interfaces;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

        public async Task<Result<int>> AddUser(User user)
        {
            if (user == null)
            {
                return new Result<int>(isSuccessful: false, $"{nameof(user)} is null");
            }

            try
            {
                var id = await _userRepository.AddUser(user);

                return new Result<int>(true, id);
            }
            catch (Exception ex)
            {
                return new Result<int>(isSuccessful:false, $"Fail to create user");
            }
        }
    }
}
