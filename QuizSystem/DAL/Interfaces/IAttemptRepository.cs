using Core.DTO;
using Core.Models;

namespace DAL.Interfaces
{
    public interface IAttemptRepository
    {
        Task<int> AddAttempt(Attempt attempt);
        Task UpdateAttempt(Attempt attempt);
       // Task DeleteAnswer(int answerId);
        Task<Attempt> GetAttemptById(int attemptId);
        Task<List<Attempt>> GetUserAttempts(int userId, int testId);
        Task<Dictionary<int, int>> GetUserTestAttemptsId(int userId);
        Task<List<Attempt>> GetAttempts(int testId, int userId);
        Task<StatisticAttemptsDTO> GetAttemptsStatistic(int testId, int userId);
        Task<double> GetAttemptAccuracy(int attemptId);
        Task<int> UserAttemptsCount(int sharedTestId, int userId);
        Task DeleteAttemptsByTest(int testId);
        Task<List<int>> GetAttemptIdByTest(int testId);

    }
}
