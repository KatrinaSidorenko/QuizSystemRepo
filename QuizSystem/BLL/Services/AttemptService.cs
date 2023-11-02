using BLL.Interfaces;
using Core.Models;
using DAL.Interfaces;
using DAL.Repository;

namespace BLL.Services
{
    public class AttemptService : IAttemptService
    {
        private readonly IAttemptRepository _attemptRepository;
        public AttemptService(IAttemptRepository attemptRepository)
        {
            _attemptRepository = attemptRepository;
        }

        public async Task<Result<int>> AddAttempt(Attempt attempt)
        {
            if (attempt == null)
            {
                return new Result<int>(isSuccessful: false, $"{nameof(attempt)} is null");
            }

            try
            {
                var id = await _attemptRepository.AddAttempt(attempt);

                return new Result<int>(true, id);
            }
            catch (Exception ex)
            {
                return new Result<int>(isSuccessful: false, $"Fail to create {nameof(attempt)} ");
            }
        }
    }
}
