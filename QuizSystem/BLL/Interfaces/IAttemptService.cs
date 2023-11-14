using Core.DTO;
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
        Task<Result<Attempt>> SaveAttemptData(AttemptResultDTO attemptResultDTO);
        Task<Result<Attempt>> GetAttemptById(int attemptId);
        Task<Result<bool>> SaveUserGivenAnswers(List<Answer> givenAnswers, int attemptId);
        Task<Result<Dictionary<int, int>>> GetUserTestAttemptsId(int userId);
        Task<Result<List<Attempt>>> GetUserTestAttempts(int testId, int userId);
        Task<Result<StatisticAttemptsDTO>> GetTestAttemptsStatistic(int testId, int userId);
        Task<Result<double>> GetAttemptAccuracy(int attemptId);
        Task<Result<(string, string)>> GetAttemptDocumentPath(int attemptId);
        Task<Result<bool>> DeleteTestWithAttempts(int testId);
    }
}
