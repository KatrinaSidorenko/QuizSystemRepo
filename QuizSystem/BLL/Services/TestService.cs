using BLL.Interfaces;
using Core.DocumentModels;
using Core.DTO;
using Core.Enums;
using Core.Models;
using Core.Settings;
using DAL.Interfaces;
using DAL.Repository;
using Microsoft.Extensions.Options;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Data.SqlClient;
using System.Text;

namespace BLL.Services
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly Core.Settings.DocumentSettings _documentSettings;
        private readonly IAnswerService _answerService;
        private readonly IQuestionService _questionService;
        public TestService(ITestRepository testRepository, IOptions<Core.Settings.DocumentSettings> options, 
            IAnswerService answerService, IQuestionService questionService)
        {
            _testRepository = testRepository;
            _answerService = answerService;
            _questionService = questionService;
            _documentSettings = options.Value;
        }

        public async Task<Result<(List<Test>, int)>> GetAllPublicTests(SortingParam sortingParam, int pageNumber = 1, int pageSize = 6,  string search = "")
        {
            try
            {
                string orderByProp = "test_id";
                string sortOrder = "acs";

                if (SortingDictionnary.SortingValues.ContainsKey(sortingParam))
                {
                    orderByProp = SortingDictionnary.SortingValues[sortingParam].prop;
                    sortOrder = SortingDictionnary.SortingValues[sortingParam].order;
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

        public async Task<Result<(List<Test>, int)>> GetUserTests(int userId, SortingParam sortingParam, Visibility? filterParam = null, int pageNumber = 1, int pageSize = 6, string search = "")
        {
            try
            {
                string orderByProp = "test_id";
                string sortOrder = "acs";

                if (SortingDictionnary.SortingValues.ContainsKey(sortingParam))
                {
                    orderByProp = SortingDictionnary.SortingValues[sortingParam].prop;
                    sortOrder = SortingDictionnary.SortingValues[sortingParam].order;
                }

                var tests = await _testRepository.GetAllUserTests(userId, filterParam, pageNumber, pageSize, orderByProp, sortOrder, search);

                return new Result<(List<Test>, int)>(true, tests);
            }
            catch (Exception ex)
            {
                return new Result<(List<Test>, int)>(isSuccessful: false, "Fail to get tests");
            }
        }

        public async Task<Result<(List<TestActivityDTO>, int)>> GetUserActivityTests(int userId, SortingParam sortingParam, Visibility? filterParam = null, int pageNumber = 1, int pageSize = 6, string search = "")
        {
            try
            {
                string orderByProp = "test_id";
                string sortOrder = "acs";

                if (SortingDictionnary.SortingValues.ContainsKey(sortingParam))
                {
                    orderByProp = SortingDictionnary.SortingValues[sortingParam].prop;
                    sortOrder = SortingDictionnary.SortingValues[sortingParam].order;
                }

                var tests = await _testRepository.GetUserActivityTests(userId, filterParam, pageNumber, pageSize, orderByProp, sortOrder, search);

                return new Result<(List<TestActivityDTO>, int)>(true, tests);
            }
            catch (Exception ex)
            {
                return new Result<(List<TestActivityDTO>, int)>(isSuccessful: false, "Fail to get tests");
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
                var result = await _testRepository.GetQuestionAmountAndPoints(testId);

                return new Result<(int, double)>(true, result);
            }
            catch (Exception ex)
            {
                return new Result<(int, double)>(false, "Fail to get questions amount");
            }
        }

        public async Task<Result<(string, string)>> GetTestDocumentPath(int testId)
        {
            //craete the member view model
            var documentModelResult = await GetTestDocumentModel(testId);

            if (!documentModelResult.IsSuccessful)
            {
                return new Result<(string, string)>(false, documentModelResult.Message);
            }

            //create the documentService
            var documentService = new DocumentService(documentModelResult.Data);

            //carete file name na dresturn it
            var fileName = CreateFileName(testId);

            var filePath = CraeteFilePath(fileName); 

            try
            {
                Settings.License = LicenseType.Community;
                Document.Create(documentService.Compose).GeneratePdf(filePath);

                return new Result<(string, string)>(true, data: (fileName, filePath));
            }
            catch (Exception ex)
            {
                return new Result<(string, string)>(false, "Fail to create document");
            }
        }

        private string CraeteFilePath(string fileName)
        {
            return Path.Combine(_documentSettings.SavingPath, fileName);
        }

        private string CreateFileName(int testId)
        {
            StringBuilder fileName = new StringBuilder($"test_{testId}");

            int i = 1;
            while (DoesTheFileExist(_documentSettings.SavingPath, fileName.ToString() + ".pdf"))
            {
                fileName.Append($"_({i})");
                i++;
            }

            return fileName.ToString()+ ".pdf";
        }

        private bool DoesTheFileExist(string folderPath, string fileName)
        {
            string fullPath = Path.Combine(folderPath, fileName);

            return File.Exists(fullPath);
        }
        private async Task<Result<TestDocumentModel>> GetTestDocumentModel(int testId)
        {
            try
            {
                var documentModel = new TestDocumentModel();
                documentModel.CreatedAt = DateTime.Now;
                var testResult = await _testRepository.GetTestById(testId);
                documentModel.Description = testResult.Description;
                documentModel.Name = testResult.Name;
                var questionAmountAndMaxMark = await GetQuestionsAmountAndMaxMark(testId);

                if (!questionAmountAndMaxMark.IsSuccessful)
                {
                    return new Result<TestDocumentModel>(false, "Fail to create the document model");
                }

                documentModel.MaxMark = questionAmountAndMaxMark.Data.Item2;
                documentModel.QuestionsAmount = questionAmountAndMaxMark.Data.Item1;

                var testQuestionsResult = await _questionService.GetTestQuestions(testId);

                if (!testQuestionsResult.IsSuccessful)
                {
                    return new Result<TestDocumentModel>(false, "Fail to create the document model");
                }

                var questions = testQuestionsResult.Data.Select(async q =>
                {
                    var model = new QuestionDocumentModel()
                    {
                        Point = q.Point,
                        Type = q.Type,
                        Description = q.Description
                    };

                    var answersResult = await _answerService.GetQuestionAnswers(q.QuestionId);

                    var answers = answersResult.Data.Select(a =>
                    {
                        var answer = new AnswerDocumentModel()
                        {
                            Value = a.Value,
                        };

                        return answer;
                    });

                    model.Answers = answers.ToList();

                    return model;
                });

                documentModel.Questions = Task.WhenAll(questions).Result.ToList();

                return new Result<TestDocumentModel>(true, documentModel);

            }
            catch (Exception ex)
            {
                return new Result<TestDocumentModel>(false, "Fail to create the document model");
            }
        }
    }
}
