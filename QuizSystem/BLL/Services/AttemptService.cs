﻿using BLL.Interfaces;
using Core.DTO;
using Core.Enums;
using Core.Models;
using DAL.Interfaces;
using DAL.Repository;
using QuestPDF.Infrastructure;
using QuestPDF;
using System.Text;
using QuestPDF.Fluent;
using Core.DocumentModels;
using Microsoft.Extensions.Options;

namespace BLL.Services
{
    public class AttemptService : IAttemptService
    {
        private readonly IAttemptRepository _attemptRepository;
        private readonly IAnswerService _answerService;
        private readonly IQuestionService _questionService;
        private readonly ITestResultService _testResultService;
        private readonly Core.Settings.DocumentSettings _documentSettings;
        private readonly ITestService _testService;
        public AttemptService(IAttemptRepository attemptRepository, IAnswerService answerService, IOptions<Core.Settings.DocumentSettings> options,
            IQuestionService questionService, ITestResultService testResultService, ITestService testService)
        {
            _attemptRepository = attemptRepository;
            _answerService = answerService;
            _documentSettings = options.Value;
            _questionService = questionService;
            _testResultService = testResultService;
            _testService = testService;
        }

        public async Task<Result<int>> AddAttempt(Attempt attempt)
        {
            if (attempt == null)
            {
                return new Result<int>(isSuccessful: false, $"{nameof(attempt)} is null");
            }

            try
            {
                var id = await _attemptRepository.AddAttempt(attempt);

                return new Result<int>(true, id);
            }
            catch (Exception ex)
            {
                return new Result<int>(isSuccessful: false, $"Fail to create {nameof(attempt)} ");
            }
        }

        public async Task<Result<Attempt>> SaveAttemptData(AttemptResultDTO attemptResultDTO)
        {
            if (attemptResultDTO == null)
            {
                return new Result<Attempt>(isSuccessful: false, $"{nameof(attemptResultDTO)} is null");
            }

            try
            {
                var attempt = await _attemptRepository.GetAttemptById(attemptResultDTO.AttemptId);
                attempt.EndDate = DateTime.Now;
                attempt.SharedTestId = attemptResultDTO.SharedTestId;

                var rightAnswers = await _questionService.GetTestQuestionsWithRightAnswers(attemptResultDTO.TestId);

                foreach(var answer in attemptResultDTO.Answers)
                {
                    if(rightAnswers.Data.TryGetValue(answer.QuestionId, out var answerIds))
                    {
                        if(answerIds.Contains(answer.AnswerId))
                        {
                            var question = await _questionService.GetQuestionById(answer.QuestionId);

                            if (question.Data.Type.Equals(QuestionType.Open))
                            {
                                var openAnswer = await _answerService.GetQuestionAnswers(question.Data.QuestionId);

                                if(openAnswer.Data.FirstOrDefault().Value.ToLower().Equals(answer.Value?.ToLower() ?? ""))
                                {
                                    attempt.Points += question.Data.Point;
                                    attempt.RightAnswersAmount++;
                                }
                            }
                            else
                            {
                                attempt.Points += (double)question.Data.Point / answerIds.Count;
                                attempt.RightAnswersAmount++;
                            }                         
                        }
                    }
                }

                await _attemptRepository.UpdateAttempt(attempt);
                var updatedAttempt = await _attemptRepository.GetAttemptById(attemptResultDTO.AttemptId);

                return new Result<Attempt>(true, updatedAttempt);
            }
            catch (Exception ex)
            {
                return new Result<Attempt>(isSuccessful: false, $"{nameof(attemptResultDTO)} is null");
            }
        }
        public async Task<Result<Attempt>> GetAttemptById(int attemptId)
        {
            try
            {
                var attempt = await _attemptRepository.GetAttemptById(attemptId);

                if (attempt == null)
                {
                    return new Result<Attempt>(isSuccessful: false, "Fail to get question");
                }

                return new Result<Attempt>(true, attempt);
            }
            catch (Exception ex)
            {
                return new Result<Attempt>(isSuccessful: false, "Fail to get question");
            }
        }

        public async Task<Result<bool>> SaveUserGivenAnswers(List<Answer> givenAnswers, int attemptId)
        {
            if (!givenAnswers.Any())
            {
                return new Result<bool>(false, "No answers");
            }

            List<TestResult> testResults = new();

            foreach (var answer in givenAnswers)
            {
                testResults.Add(new TestResult()
                {
                    AnswerId = answer.AnswerId,
                    QuestionId = answer.QuestionId,
                    AttemptId = attemptId,
                    EnteredValue = answer.Value
                });
            }

            try
            {
                var saveResult = await _testResultService.AddRangeOfTestResults(testResults);

                if (!saveResult.IsSuccessful)
                {
                    return new Result<bool>(false, "Fail to save answers");
                }

                return new Result<bool>(true);
            }
            catch(Exception ex)
            {
                return new Result<bool>(false, "Fail to save answers");
            }
        }

        public async Task<Result<Dictionary<int, int>>> GetUserTestAttemptsId(int userId)
        {
            try
            {
                var result = await _attemptRepository.GetUserTestAttemptsId(userId);

                return new Result<Dictionary<int, int>>(true, result);
            }
            catch (Exception ex)
            {
                return new Result<Dictionary<int, int>>(false, "Fail to get test Ids");
            }
        }

        public async Task<Result<List<Attempt>>> GetUserTestAttempts(int testId, int userId)
        {
            try
            {
                var attempts = await _attemptRepository.GetAttempts(testId, userId);

                return new Result<List<Attempt>>(true, attempts);
            }
            catch(Exception ex)
            {
                return new Result<List<Attempt>>(false, "Fail to get attempts");
            }
        }

        public async Task<Result<double>> GetAttemptAccuracy(int attemptId)
        {
            try
            {
                var accuracy = await _attemptRepository.GetAttemptAccuracy(attemptId);

                return new Result<double>(true, accuracy);
            }
            catch (Exception ex)
            {
                return new Result<double>(false, "Fail to get accuracy");
            }
        }

        public async Task<Result<StatisticAttemptsDTO>> GetTestAttemptsStatistic(int testId, int userId)
        {
            try
            {
                var statistic = await _attemptRepository.GetAttemptsStatistic(testId, userId);

                return new Result<StatisticAttemptsDTO>(true, statistic);
            }
            catch (Exception ex)
            {
                return new Result<StatisticAttemptsDTO>(false, "Fail to get statistic");
            }
        }

        public async Task<Result<(string, string)>> GetAttemptDocumentPath(int attemptId)
        {
            var documentModelResult = await GetAttemptDocumentModel(attemptId);

            if (!documentModelResult.IsSuccessful)
            {
                return new Result<(string, string)>(false, documentModelResult.Message);
            }
            //create the documentService
            var documentService = new AttemptDocumentService(documentModelResult.Data);

            //carete file name na dresturn it
            var fileName = CreateFileName(attemptId);

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

        public async Task<Result<bool>> DeleteTestWithAttempts(int testId)
        {
            try
            {
                var attemptsIds = await _attemptRepository.GetAttemptIdByTest(testId);
                var deleteResult = await _testResultService.DeleteRangeOfTestResults(attemptsIds);
                //delete attempts
                
                if (!deleteResult.IsSuccessful)
                {
                    return new Result<bool>(false, deleteResult.Message);
                }

                await _attemptRepository.DeleteAttemptsByTest(testId);

                await _testService.DeleteTest(testId);
                return new Result<bool>(isSuccessful: true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false, "Fail to delete test and attempts");
            }
        }

        private string CraeteFilePath(string fileName)
        {
            return Path.Combine(_documentSettings.SavingPath, fileName);
        }

        private string CreateFileName(int attemptId)
        {
            StringBuilder fileName = new StringBuilder($"attempt_{attemptId}");

            int i = 1;
            while (DoesTheFileExist(_documentSettings.SavingPath, fileName.ToString() + ".pdf"))
            {
                fileName.Append($"_({i})");
                i++;
            }

            return fileName.ToString() + ".pdf";
        }

        private bool DoesTheFileExist(string folderPath, string fileName)
        {
            string fullPath = Path.Combine(folderPath, fileName);

            return File.Exists(fullPath);
        }
        private async Task<Result<AttemptResultDocumentDTO>> GetAttemptDocumentModel(int attemptId)
        {
            try
            {
                var attemptResult = await GetAttemptById(attemptId);

                if (!attemptResult.IsSuccessful)
                {
                    return new Result<AttemptResultDocumentDTO>(false, attemptResult.Message);
                }


                var attemptDocumentModel = new AttemptResultDocumentDTO();
                var testResult = await _testService.GetTestById(attemptResult.Data.TestId);

                attemptDocumentModel.TestId = testResult.Data.TestId;
                attemptDocumentModel.Name = testResult.Data.Name;
                attemptDocumentModel.AttemptId = attemptId;
                attemptDocumentModel.Mark = attemptResult.Data.Points;

                var questionsResult = await _questionService.GetTestQuestions(testResult.Data.TestId);

                var questionsVM = questionsResult.Data.Select(async q =>
                {
                    var answers = await _answerService.GetQuestionAnswers(q.QuestionId);
                    

                    var answersVm = answers.Data.Select(async a =>
                    {
                        var testResult = await _testResultService.GetTestResult(attemptId, q.QuestionId, a.AnswerId);
                        var attemptAnswer = new AttemptAnswerDocumentDTO()
                        {
                            AnswerId = a.AnswerId,
                            IsRight = a.IsRight,
                            Value = a.Value
                        };


                        if (q.Type.Equals(QuestionType.Open))
                        {
                            attemptAnswer.ChoosenByUser = testResult.Data.EnteredValue.ToLower().Equals(a.Value) ? true : false;
                            attemptAnswer.ValueByUser = testResult.Data.EnteredValue;
                        }
                        else
                        {
                            attemptAnswer.ChoosenByUser = a.AnswerId == testResult.Data.AnswerId ? true : false;
                        }


                        return attemptAnswer;

                    }).ToList();

                    var question = new AttemptQuestionDocumentDTO()
                    {
                        QuestionId = q.QuestionId,
                        Description = q.Description,
                        //GetedPoints = ((float)q.Point / answers.Data.Count) * (float)answersVm.Select(a => a.ChoosenByUser && a.IsRight == true).Count(),
                        Point = (float)q.Point,
                        Type = q.Type,
                        Answers = Task.WhenAll(answersVm).Result.ToList()
                    };

                    return question;

                }).ToList();

                attemptDocumentModel.Questions = Task.WhenAll(questionsVM).Result.ToList();

                return new Result<AttemptResultDocumentDTO>(true, attemptDocumentModel);

            }
            catch (Exception ex)
            {
                return new Result<AttemptResultDocumentDTO>(false, "Fail to create the document model");
            }
        }

    }
}
