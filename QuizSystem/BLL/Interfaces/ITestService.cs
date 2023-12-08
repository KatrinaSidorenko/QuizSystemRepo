using Core.DTO;
using Core.Enums;
using Core.Models;

namespace BLL.Interfaces
{
    public interface ITestService
    {
        Task<Result<int>> AddTest(Test test);
        Task<Result<bool>> DeleteTest(int testId);
        Task<Result<(List<Test>, int)>> GetAllPublicTests(SortingParam sortingParam, int pageNumber = 1, int pageSize = 6, string search = "");
        Task<Result<Test>> GetTestById(int testId);
        Task<Result<(List<TestActivityDTO>, int)>> GetUserActivityTests(int userId, SortingParam sortingParam, Visibility? filterParam = null, int pageNumber = 1, int pageSize = 6, string search = "");
        Task<Result<(List<Test>, int)>> GetUserTests(int userId, SortingParam sortingParam, Visibility? filterParam = null, int pageNumber = 1, int pageSize = 6, string search = "");
        Task<Result<bool>> UpdateTest(Test test);
        Task<Result<bool>> IsUserTest(int testId, int userId);
        Task<Result<List<Test>>> GetRangeOfTests(List<int> testIds);
        Task<Result<Dictionary<int, int>>> GetTestAttemptsCount();
        Task<Result<(int, double)>> GetQuestionsAmountAndMaxMark(int testId);
        Task<Result<(string, string)>> GetTestDocumentPath(int testId);
        Task<Result<bool>> IsInTestQuestions(int testId);
    }
}
