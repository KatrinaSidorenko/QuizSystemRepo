using Core.DTO;
using Core.Enums;
using Core.Models;

namespace BLL.Interfaces
{
    public interface ISharedTestService
    {
        Task<Result<int>> AddSharedTest(SharedTest sharedTest);
        Task<Result<bool>> DeleteSharedTest(int sharedTestId);
        Task<Result<SharedTest>> GetSharedTestById(int sharedTestId);
        Task<Result<List<SharedTest>>> GetSharedTests();
        Task<Result<bool>> UpdateSharedTest(SharedTest sharedTest);
        Task<Result<SharedTest>> GetSharedTestByCode(Guid code, int userId);
        Task<Result<(List<SharedTestDTO>, int)>> GetUserSharedTests(int userId, SortingParam sortingParam, SharedTestStatus? filterParam = null, int pageNumber = 1, int pageSize = 6, string search = "");
        Task<Result<bool>> FinishSharedTest(int sharedTestId);
        Task<Result<bool>> IsTestShared(int testId);
        Task<Result<SharedTestStatDTO>> GetSharedTestStatistic(int sharedTestId);
    }
}