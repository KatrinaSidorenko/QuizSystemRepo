using BLL.Interfaces;
using Core.DTO;
using Core.Enums;
using Core.Models;
using DAL.Interfaces;
using System.Reflection;

namespace BLL.Services
{
    public class SharedTestService : ISharedTestService
    {
        private readonly ISharedTestRepository _sharedTestRepository;
        private readonly IAttemptRepository _attemptRepository;

        public SharedTestService(ISharedTestRepository sharedTestRepository, IAttemptRepository attemptRepository)
        {
            _sharedTestRepository = sharedTestRepository;
            _attemptRepository = attemptRepository;
        }

        public async Task<Result<int>> AddSharedTest(SharedTest sharedTest)
        {
            if (sharedTest == null)
            {
                return new Result<int>(isSuccessful: false, $"{nameof(sharedTest)} is null");
            }

            try
            {
                SetSharedTestStatus(sharedTest);

                var code = Guid.NewGuid();
                sharedTest.TestCode = code;

                var id = await _sharedTestRepository.AddSharedTest(sharedTest);

                return new Result<int>(true, id);
            }
            catch (Exception ex)
            {
                return new Result<int>(isSuccessful: false, $"Fail to create {nameof(sharedTest)}");
            }
        }

        public async Task<Result<bool>> DeleteSharedTest(int sharedTestId)
        {
            try
            {
                await _sharedTestRepository.DeleteSharedTest(sharedTestId);

                return new Result<bool>(isSuccessful: true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false, "Fail to delete shared test");
            }
        }

        public async Task<Result<SharedTest>> GetSharedTestById(int sharedTestId)
        {
            try
            {
                var sharedTest = await _sharedTestRepository.GetSharedTestById(sharedTestId);

                if (sharedTest == null)
                {
                    return new Result<SharedTest>(isSuccessful: false, $"Fail to get {nameof(sharedTest)}");
                }

                return new Result<SharedTest>(true, sharedTest);
            }
            catch (Exception ex)
            {
                return new Result<SharedTest>(isSuccessful: false, $"Fail to get shared test");
            }
        }

        public async Task<Result<List<SharedTest>>> GetSharedTests()
        {
            try
            {
                var sharedTests = await _sharedTestRepository.GetTests();

                return new Result<List<SharedTest>>(true, sharedTests);
            }
            catch (Exception ex)
            {
                return new Result<List<SharedTest>>(isSuccessful: false, "Fail to get shared tests");
            }
        }

        public async Task<Result<bool>> UpdateSharedTest(SharedTest sharedTest)
        {
            if (sharedTest == null)
            {
                return new Result<bool>(isSuccessful: false, $"Fail to update {nameof(sharedTest)}");
            }

            try
            {
                await _sharedTestRepository.UpdateSharedTest(sharedTest);

                return new Result<bool>(isSuccessful: true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false, "Fail to update shared test");
            }
        }

        public async Task<Result<SharedTest>> GetSharedTestByCode(Guid code, int userId)
        {

            try
            {
                var codeExist = await _sharedTestRepository.IsCodeExist(code);

                if (!codeExist)
                {
                    return new Result<SharedTest>(isSuccessful: false, $"Invalid code");
                }

                var sharedTest = await _sharedTestRepository.GetSharedTestByCode(code);

                if (sharedTest.Status.Equals(SharedTestStatus.Completed) || sharedTest.EndDate <= DateTime.Now)
                {
                    return new Result<SharedTest>(isSuccessful: false, $"This test was alresdy finished");
                }

                if (sharedTest.StartDate >= DateTime.Now || sharedTest.Status.Equals(SharedTestStatus.Planned))
                {
                    return new Result<SharedTest>(isSuccessful: false, $"This test not strated yet");
                }

                var attemptsCount = await _attemptRepository.UserAttemptsCount(sharedTest.SharedTestId, userId);

                if(!(attemptsCount < sharedTest.AttemptCount))
                {
                    return new Result<SharedTest>(isSuccessful: false, $"Your attempts are finished");
                }

                if (sharedTest == null)
                {
                    return new Result<SharedTest>(isSuccessful: false, $"Fail to get {nameof(sharedTest)}");
                }

                return new Result<SharedTest>(true, sharedTest);
            }
            catch (Exception ex)
            {
                return new Result<SharedTest>(isSuccessful: false, $"Fail to get shared test");
            }
        }

        public async Task<Result<(List<SharedTestDTO>, int)>> GetUserSharedTests(int userId, SortingParam sortingParam, int pageNumber = 1, int pageSize = 6, string search = "")
        {

            try
            {
                string orderByProp = "shared_test_id";
                string sortOrder = "asc";

                if (SortingDictionnary.SortingValues.ContainsKey(sortingParam))
                {
                     orderByProp = SortingDictionnary.SortingValues[sortingParam].prop;
                     sortOrder = SortingDictionnary.SortingValues[sortingParam].order;
                }
                
                var sharedTest = await _sharedTestRepository.GetUserSharedTestsWithTotalRecords(userId, pageNumber, pageSize, orderByProp, sortOrder);

                if (sharedTest.Item1 == null)
                {
                    return new Result<(List<SharedTestDTO>, int)>(isSuccessful: false, $"Fail to get {nameof(sharedTest)}");
                }

                List<SharedTestDTO> result = sharedTest.Item1;

                if (!string.IsNullOrEmpty(search))
                {
                    result = sharedTest.Item1.Where(t => t.TestName.ToLower().Contains(search)).ToList();
                }
                                
                return new Result<(List<SharedTestDTO>, int)>(true, (result, sharedTest.Item2));
            }
            catch (Exception ex)
            {
                return new Result<(List<SharedTestDTO>, int)>(isSuccessful: false, $"Fail to get shared test");
            }
        }

        public async Task<Result<bool>> FinishSharedTest(int sharedTestId)
        {
            try
            {
                var sharedTest = await _sharedTestRepository.GetSharedTestById(sharedTestId);

                if(sharedTest == null)
                {
                    return new Result<bool>(isSuccessful: false, $"Fail to get {nameof(sharedTest)}");
                }

                sharedTest.Status = SharedTestStatus.Completed;

                await _sharedTestRepository.UpdateSharedTest(sharedTest);

                return new Result<bool>(isSuccessful: true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false, $"Fail to update shared test");
            }
        }

        public async Task<Result<bool>> IsTestShared(int testId)
        {
            try
            {
                var result = await _sharedTestRepository.IsTestShared(testId);

                if (result)
                {
                    return new Result<bool>(false, "This test already shared");
                }

                return new Result<bool>(true, result);
            }
            catch (Exception ex)
            {
                return new Result<bool>(false, "Fail to check shared test");
            }
        }
        private void SetSharedTestStatus(SharedTest sharedTest)
        {
            if (sharedTest.StartDate > DateTime.Now)
            {
                sharedTest.Status = SharedTestStatus.Planned;
            }

            if (sharedTest.StartDate <= DateTime.Now)
            {
                sharedTest.Status = SharedTestStatus.InProgress;
            }

            if (sharedTest.EndDate < DateTime.Now)
            {
                sharedTest.Status = SharedTestStatus.Completed;
            }
        }


    }
}
