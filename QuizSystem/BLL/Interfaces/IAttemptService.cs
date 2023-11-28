using Core.DTO;
using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAttemptService
    {
        Task<Result<int>> AddAttempt(Attempt attempt);
        Task<Result<int>> SaveAttemptData(AttemptResultDTO attemptResultDTO);
        Task<Result<Attempt>> GetAttemptById(int attemptId);
        Task<Result<bool>> SaveUserGivenAnswers(List<Answer> givenAnswers, int attemptId);
        Task<Result<Dictionary<int, int>>> GetUserTestAttemptsId(int userId);
        Task<Result<(List<AttemptHistoryDTO>, int)>> GetUserTestAttempts(int testId, int userId, SortingParam sortingParam, int? sharedTestId = null, int pageNumber = 1, int pageSize = 6, string search = "", int startAccuracy = 0, int endAccuracy = 100,
            DateTime? startDate = null, DateTime? endDate = null);
        Task<Result<(List<SharedAttemptDTO>, int)>> GetSharedAttempts(int sharedTestId, SortingParam sortingParam, int pageNumber = 1, int pageSize = 6, string search = "");
        Task<Result<StatisticAttemptsDTO>> GetTestAttemptsStatistic(int testId, int userId);
        Task<Result<double>> GetAttemptAccuracy(int attemptId);
        Task<Result<(string, string)>> GetAttemptDocumentPath(int attemptId);
        Task<Result<bool>> DeleteTestWithAttempts(int testId);
    }
}
