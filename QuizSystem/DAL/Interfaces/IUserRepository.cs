using Core.Models;

namespace DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsers();
        Task AddUser(User user);
        Task<User> GetUserById(int id);
        Task UpdateUser(User user);
        Task DeleteUser(int userId);
    }
}
