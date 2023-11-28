using Core.DTO;
using Core.Enums;
using Core.Models;

namespace DAL.Interfaces
{
    public interface ISharedTestRepository
    {
        Task<int> AddSharedTest(SharedTest sharedTest);
        Task DeleteSharedTest(int sharedTestId);
        Task<SharedTest> GetSharedTestById(int sharedTestId);
        Task<List<SharedTest>> GetTests();
        Task UpdateSharedTest(SharedTest sharedTest);
        Task<SharedTest> GetSharedTestByCode(Guid code);
        Task<List<SharedTestDTO>> GetUserSharedTests(int userId);
        Task<bool> IsTestShared(int testId);
        Task<bool> IsCodeExist(Guid code);
        Task<List<UserStatDTO>> UsersStatistic(int sharedTestId);
        Task<(List<SharedTestDTO>, int)> GetUserSharedTestsWithTotalRecords(int userId, int pageNumber = 1, int pageSize = 6, string orderByProp = "shared_test_id", string sortOrder = "asc", SharedTestStatus? filterParam = null);
    }
}