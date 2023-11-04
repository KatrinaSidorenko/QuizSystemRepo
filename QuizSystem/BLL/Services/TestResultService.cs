using BLL.Interfaces;
using Core.Models;
using DAL.Interfaces;
using DAL.Repository;

namespace BLL.Services
{
    public class TestResultService : ITestResultService
    {
        private readonly ITestResultRepository _testResultRepository;
        public TestResultService(ITestResultRepository testResultRepository)
        {
            _testResultRepository = testResultRepository;
        }

        public async Task<Result<bool>> AddRangeOfAnswers(List<TestResult> testResults)
        {
            if (!testResults.Any())
            {
                return new Result<bool>(false);
            }

            try
            {
                var tasks = new List<Task>();

                foreach (var testResult in testResults)
                {
                    tasks.Add(_testResultRepository.AddTestResult(testResult));
                }

                await Task.WhenAll(tasks);

                return new Result<bool>(true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(false, "Fail to add test result");
            }
        }
    }
}
