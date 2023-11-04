using Core.Models;

namespace BLL.Interfaces
{
    public interface ITestResultService
    {
        Task<Result<bool>> AddRangeOfAnswers(List<TestResult> testResults);
    }
}