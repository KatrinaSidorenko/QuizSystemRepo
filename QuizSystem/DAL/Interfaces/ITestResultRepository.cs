using Core.Models;

namespace DAL.Interfaces
{
    public interface ITestResultRepository
    {
        Task<int> AddTestResult(TestResult testResult);
        Task<TestResult> GetTestResultByAttemptIdandQuestionId(int attemptId, int questionId, int answerId);
        Task DeleteTestResultByAttempt(int attemptId);
    }
}