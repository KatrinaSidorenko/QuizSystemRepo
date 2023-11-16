using Core.Models;

namespace BLL.Interfaces
{
    public interface ITestResultService
    {
        Task<Result<bool>> AddRangeOfTestResults(List<TestResult> testResults);
        Task<Result<TestResult>> GetTestResult(int attemptId, int questionId, int answerId);
        Task<Result<bool>> DeleteRangeOfTestResults(List<int> attemptIds);
    }
}