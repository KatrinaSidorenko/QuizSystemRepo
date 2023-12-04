using Core.Models;

namespace DAL.Interfaces
{
    public interface ITestResultRepository
    {
        Task<int> AddTestResult(TestResult testResult);
        Task<TestResult> GetTestResultByAttemptIdandQuestionId(int attemptId, int questionId, int answerId);
        Task DeleteTestResultByAttempt(int attemptId);
        Task<(double sum, int rightAmount)> GetAttemptPointsData(int attemptId);
        Task DeleteTestResultByQuestion(int questionId);
    }
}