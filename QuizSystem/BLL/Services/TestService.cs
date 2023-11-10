﻿using BLL.Interfaces;
using Core.Enums;
using Core.Models;
using DAL.Interfaces;
using DAL.Repository;

namespace BLL.Services
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;
        public TestService(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        public async Task<Result<(List<Test>, int)>> GetAllPublicTests(SortingParam sortingParam, int pageNumber = 1, int pageSize = 6,  string search = "")
        {
            try
            {
                string orderByProp = "test_id";
                string sortOrder = "acs";

                switch (sortingParam)
                {
                    case SortingParam.Name:
                        orderByProp = "test_name";
                        sortOrder = "acs";
                        break;
                    case SortingParam.NameDesc:
                        orderByProp = "test_name";
                        sortOrder = "desc";
                        break;
                    case SortingParam.Date:
                        orderByProp = "date_of_creation";
                        sortOrder = "acs";
                        break;
                    case SortingParam.DateDesc:
                        orderByProp = "date_of_creation";
                        sortOrder = "desc";
                        break;
                }

                   
                var pablicTestsAndRecordsAmount = await _testRepository.GetAllPublicTestsWithTotalRecords(pageNumber, pageSize, orderByProp, sortOrder);

                var result = pablicTestsAndRecordsAmount.Item1.Where(t => t.Name.ToLower().Contains(search)).ToList();

                return new Result<(List<Test>, int)>(true, (result, pablicTestsAndRecordsAmount.Item2));
            }
            catch (Exception ex)
            {
                return new Result<(List<Test>, int)>(isSuccessful: false, "Fail to get public tests");
            }
        }

        public async Task<Result<List<Test>>> GetUserTests(int userId)
        {
            try
            {
                var tests = await _testRepository.GetUserTests(userId);

                return new Result<List<Test>>(true, tests);
            }
            catch (Exception ex)
            {
                return new Result<List<Test>>(isSuccessful: false, "Fail to get tests");
            }
        }

        public async Task<Result<int>> AddTest(Test test)
        {
            if (test == null)
            {
                return new Result<int>(isSuccessful: false, $"{nameof(test)} is null");
            }

            try
            {
                var id = await _testRepository.AddTest(test);

                return new Result<int>(true, id);
            }
            catch (Exception ex)
            {
                return new Result<int>(isSuccessful: false, $"Fail to create {nameof(test)} ");
            }
        }

        public async Task<Result<bool>> DeleteTest(int testId)
        {
            try
            {
                await _testRepository.DeleteTest(testId);

                return new Result<bool>(isSuccessful: true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false, "Fail to delete test");
            }
        }

        public async Task<Result<Test>> GetTestById(int testId)
        {
            try
            {
                var test = await _testRepository.GetTestById(testId);

                if (test == null)
                {
                    return new Result<Test>(isSuccessful: false, $"Fail to get {nameof(test)}");
                }

                return new Result<Test>(true, test);
            }
            catch (Exception ex)
            {
                return new Result<Test>(isSuccessful: false, $"Fail to get test");
            }
        }

        public async Task<Result<bool>> UpdateTest(Test test)
        {
            if (test == null)
            {
                return new Result<bool>(isSuccessful: false, $"Fail to update {nameof(test)}");
            }

            try
            {
                await _testRepository.UpdateTest(test);

                return new Result<bool>(isSuccessful: true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false, "Fail to update test");
            }
        }

        public async Task<Result<bool>> IsUserTest(int testId, int userId)
        {
            try
            {
                var tests = await _testRepository.GetAllTests();

                var test = tests.FirstOrDefault(t => t.UserId == userId && t.TestId == testId);

                if (test == null)
                {
                    return new Result<bool>(isSuccessful: false);
                }

                return new Result<bool>(isSuccessful: true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false);
            }
        }

        public async Task<Result<List<Test>>> GetRangeOfTests(List<int> testIds)
        {
            if (!testIds.Any())
            {
                return new Result<List<Test>>(true, new List<Test>());
            }

            try
            {
                var tasks = new List<Task<Test>>();

                testIds.ForEach(testId =>
                {
                    tasks.Add(_testRepository.GetTestById(testId));
                });


                var tests = await Task.WhenAll(tasks);

                return new Result<List<Test>>(true, tests.ToList());
            }
            catch (Exception ex)
            {
                return new Result<List<Test>>(false, "Fail to get range of tests");
            }
        }

        public async Task<Result<Dictionary<int, int>>> GetTestAttemptsCount()
        {
            try
            {
               var result = await _testRepository.GetTestAttemptsCount();

                return new Result< Dictionary<int, int> > (true, result);
            }
            catch (Exception ex)
            {
                return new Result<Dictionary<int, int> > (false, "Fail to get range of tests");
            }
        }

        public async Task<Result<(int, double)>> GetQuestionsAmountAndMaxMark(int testId)
        {
            try
            {
                var result = await _testRepository.GetQyestionAmountAndPoints(testId);

                return new Result<(int, double)>(true, result);
            }
            catch (Exception ex)
            {
                return new Result<(int, double)>(false, "Fail to get questions amount");
            }
        }
    }
}
