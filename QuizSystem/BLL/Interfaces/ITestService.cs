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
        Task<Result<List<Test>>> GetAllPublicTests();
        Task<Result<Test>> GetTestById(int testId);
        Task<Result<List<Test>>> GetUserTests(int userId);
        Task<Result<bool>> UpdateTest(Test test);
        Task<Result<bool>> IsUserTest(int testId, int userId);
        Task<Result<List<Test>>> GetRangeOfTests(List<int> testIds);
    }
}
