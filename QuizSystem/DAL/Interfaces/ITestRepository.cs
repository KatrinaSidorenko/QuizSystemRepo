using Core.Enums;
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
        Task<(List<Test>, int)> GetAllUserTests(int userId, Visibility? filterParam = null, int pageNumber = 1, int pageSize = 6, string orderByProp = "test_id", string sortOrder = "asc", string serachParam = null);
        Task<(List<Test>, int)> GetAllPublicTestsWithTotalRecords(int pageNumber = 1, int pageSize = 6, string orderByProp = "test_id", string sortOrder = "asc");
        Task<(int, double)> GetQyestionAmountAndPoints(int testId);
    }
}
