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
        Task<(List<Attempt>, int)> GetAttempts(int testId, int userId, int pageNumber = 1, int pageSize = 6, string orderByProp = "attempt_id", string sortOrder = "asc", int? sharedTestId = null, int startAccuracy = 0, int endAccuracy = 100,
            DateTime? startDate = null, DateTime? endDate = null);
        Task<(List<SharedAttemptDTO>, int)> GetSharedAttempts(int sharedTestId, int pageNumber = 1, int pageSize = 6, string orderByProp = "shared_test_id", string sortOrder = "asc");
        Task<StatisticAttemptsDTO> GetAttemptsStatistic(int testId, int userId);
        Task<double> GetAttemptAccuracy(int attemptId);
        Task<int> UserAttemptsCount(int sharedTestId, int userId);
        Task DeleteAttemptsByTest(int testId);
        Task<List<int>> GetAttemptIdByTest(int testId);
        Task<List<int>> GetAttemptIdBySharedTest(int sharedTestId);
        Task DeleteAttemptsBySharedTest(int sharedTestId);

    }
}
