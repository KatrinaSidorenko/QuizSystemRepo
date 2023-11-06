using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        Task<Result<List<User>>> GetAllUsers();
        Task<Result<int>> AddUser(User user);
        Task<Result<bool>> DeleteUser(int userId);
        Task<Result<User>> GetUserById(int userId);
        Task<Result<bool>> UpdateUser(User user);
        Task<Result<bool>> IsTheEmailAvailable(string email);
        Task<Result<User>> IsUserExist(string email, string password);
        Task<Result<bool>> IsUserExist(int userId);
    }
}
