using Core.Models;

namespace BLL.Interfaces
{
    public interface ITestResultService
    {
        Task<Result<bool>> AddRangeOfTestResults(List<TestResult> testResults);
        Task<Result<TestResult>> GetTestResult(int attemptId, int questionId, int answerId);
        Task<Result<bool>> DeleteRangeOfTestResults(List<int> attemptIds);
        Task<Result<(double sum, int rA)>> GetAttemptData(int attemptId);
        Task<Result<bool>> DeleteTestResultsByQuestion(int questionId);
        Task<Result<int>> EnteredRightAnswersAmount(int questionId, int sharedTestId);
    }
}