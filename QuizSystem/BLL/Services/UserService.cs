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

        public async Task<Result<bool>> DeleteUser(int userId)
        {
            try
            {
                await _userRepository.DeleteUser(userId);

                return new Result<bool>(isSuccessful: true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false, "Fail to delete user");
            }
        }

        public async Task<Result<User>> GetUserById(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserById(userId);

                if(user == null)
                {
                    return new Result<User>(isSuccessful: false, "Fail to get user");
                }

                return new Result<User>(true, user);
            }
            catch (Exception ex)
            {
                return new Result<User>(isSuccessful: false, "Fail to get user");
            }
        }

        public async Task<Result<bool>> UpdateUser(User user)
        {
            if (user == null)
            {
                return new Result<bool>(isSuccessful: false, "Fail to update user");
            }

            try
            {
                await _userRepository.UpdateUser(user);

                return new Result<bool>(isSuccessful: true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false, "Fail to update user");
            }
        }

        public async Task<Result<bool>> IsTheEmailAvailable(string email)
        {
            try
            {
                var allUsersResult = await GetAllUsers();

                if (!allUsersResult.IsSuccessful)
                {
                    return new Result<bool>(isSuccessful: false, "Something went wrong");
                }

                if (!allUsersResult.Data.Any())
                {
                    return new Result<bool>(isSuccessful: true);
                }

                var user = allUsersResult.Data.FirstOrDefault(u => u.Email == email);

                if (user == null)
                {
                    return new Result<bool>(isSuccessful: true);

                }

                return new Result<bool>(isSuccessful: false, "Email already exist");
            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false, "Something went wrong");
            }
        }

        public async Task<Result<User>> IsUserExist(string email, string password)
        {
            try
            {
                var allUsersResult = await GetAllUsers();

                if (!allUsersResult.IsSuccessful)
                {
                    return new Result<User>(isSuccessful: false, "Something went wrong");
                }

                if (!allUsersResult.Data.Any())
                {
                    return new Result<User>(isSuccessful: false);
                }

                var user = allUsersResult.Data.FirstOrDefault(u => u.Email == email);

                if (user != null)
                {
                    if(user.Password == password)
                    {
                        return new Result<User>(isSuccessful: true, user);

                    }
                    else
                    {
                        return new Result<User>(isSuccessful: false, "Invalid password");
                    }
                }

                return new Result<User>(isSuccessful: false, "User doesn't exist");
            }
            catch (Exception ex)
            {
                return new Result<User>(isSuccessful: false, "Something went wrong");
            }
        }
    }
}
