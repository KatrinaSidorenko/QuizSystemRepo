using BLL.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Interfaces;
using System.Reflection;

namespace BLL.Services
{
    public class SharedTestService : ISharedTestService
    {
        private readonly ISharedTestRepository _sharedTestRepository;

        public SharedTestService(ISharedTestRepository sharedTestRepository)
        {
            _sharedTestRepository = sharedTestRepository;
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

        public async Task<Result<SharedTest>> GetSharedTestByCode(Guid code)
        {

            try
            {
                var sharedTest = await _sharedTestRepository.GetSharedTestByCode(code);

                //validation

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
