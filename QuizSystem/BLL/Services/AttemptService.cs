using BLL.Interfaces;
using Core.DTO;
using Core.Enums;
using Core.Models;
using DAL.Interfaces;
using DAL.Repository;

namespace BLL.Services
{
    public class AttemptService : IAttemptService
    {
        private readonly IAttemptRepository _attemptRepository;
        private readonly IAnswerService _answerService;
        private readonly IQuestionService _questionService;
        private readonly ITestResultService _testResultService;
        public AttemptService(IAttemptRepository attemptRepository, IAnswerService answerService, 
            IQuestionService questionService, ITestResultService testResultService)
        {
            _attemptRepository = attemptRepository;
            _answerService = answerService;
            _questionService = questionService;
            _testResultService = testResultService;
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

                                if(openAnswer.Data.FirstOrDefault().Value.ToLower().Equals(answer.Value.ToLower()))
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
    }
}
