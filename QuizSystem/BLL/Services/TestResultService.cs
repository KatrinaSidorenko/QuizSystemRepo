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

        public async Task<Result<bool>> AddRangeOfTestResults(List<TestResult> testResults)
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

        public async Task<Result<TestResult>> GetTestResult(int attemptId, int questionId)
        {
            try
            {
                var teatResult = await _testResultRepository.GetTestResultByAttemptIdandQuestionId(attemptId, questionId);

                if (teatResult == null)
                {
                    return new Result<TestResult>(false, "Fail to get test reault");
                }

                return new Result<TestResult>(true, data: teatResult);
            }
            catch (Exception ex)
            {
                return new Result<TestResult>(false, "Fail to get test reault");
            }
        }

        public async Task<Result<bool>> DeleteRangeOfTestResults(List<int> attemptIds)
        {
            try
            {
                var tasks = new List<Task>();

                foreach (var attemptId in attemptIds)
                {
                    tasks.Add(_testResultRepository.DeleteTestResultByAttempt(attemptId));
                }

                await Task.WhenAll(tasks);

                return new Result<bool>(isSuccessful: true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false, "Fail to delete test");
            }
        }
    }
}
