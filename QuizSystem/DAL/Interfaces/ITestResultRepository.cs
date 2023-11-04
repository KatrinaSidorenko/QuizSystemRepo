using Core.Models;

namespace DAL.Interfaces
{
    public interface ITestResultRepository
    {
        Task<int> AddTestResult(TestResult testResult);
    }
}