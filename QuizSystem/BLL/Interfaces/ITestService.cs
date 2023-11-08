using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ITestService
    {
        Task<Result<int>> AddTest(Test test);
        Task<Result<bool>> DeleteTest(int testId);
        Task<Result<(List<Test>, int)>> GetAllPublicTests(int pageNumber = 1, int pageSize = 6, string orderByProp = "test_id", string sortOrder = "asc");
        Task<Result<Test>> GetTestById(int testId);
        Task<Result<List<Test>>> GetUserTests(int userId);
        Task<Result<bool>> UpdateTest(Test test);
        Task<Result<bool>> IsUserTest(int testId, int userId);
        Task<Result<List<Test>>> GetRangeOfTests(List<int> testIds);
        Task<Result<Dictionary<int, int>>> GetTestAttemptsCount();
    }
}
