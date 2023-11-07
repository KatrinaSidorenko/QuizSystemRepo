using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ITestRepository
    {
        Task<List<Test>> GetAllTests();
        Task<int> AddTest(Test test);
        //Task AddTest(Test test);
        Task<Test> GetTestById(int id);
        Task UpdateTest(Test test);
        Task DeleteTest(int testId);
        Task<List<Test>> GetUserTests(int userId);
        Task<Dictionary<int, int>> GetTestAttemptsCount();
    }
}
