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
        public AttemptService(IAttemptRepository attemptRepository, IAnswerService answerService, IQuestionService questionService)
        {
            _attemptRepository = attemptRepository;
            _answerService = answerService;
            _questionService = questionService;
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
    }
}
