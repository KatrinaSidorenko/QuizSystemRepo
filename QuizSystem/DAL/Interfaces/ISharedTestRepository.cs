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
    }
}