using BLL.Interfaces;
using Core.Models;
using DAL.Interfaces;

namespace BLL.Services
{
    public class QuestionService: IQuestionService
    {
        private readonly IAnswerService _answerService;
        private readonly IQuestionRepository _questionRepository;
        public QuestionService(IQuestionRepository questionRespository, IAnswerService answerService)
        {
            _answerService = answerService;
            _questionRepository = questionRespository;
        }

        public async Task<Result<Question>> AddQuestionWithAnswers(Question question, List<Answer> answers)
        {
            if(question == null || answers.Count == 0)
            {
                return new Result<Question>(false, "No answers to the question");
            }

            try
            {
                var questionId = await _questionRepository.AddQuestion(question);

                answers.ForEach(answersItem => { answersItem.QuestionId = questionId; });

                var answerResult = await _answerService.AddRangeOfAnswers(answers);

                if (!answerResult.IsSuccessful)
                {
                    return new Result<Question>(false, answerResult.Message);
                }

                return new Result<Question>(isSuccessful: true);
            }
            catch (Exception ex)
            {
                return new Result<Question>(false, "Fail to create question and answers");
            }

        }

        public async Task<Result<bool>> EditQuestionAndAnswers(Question question, List<Answer> answers)
        {
            if (question == null || answers.Count == 0)
            {
                return new Result<bool>(isSuccessful: false, "No answers to the question");
            }

            try
            {
                await _questionRepository.UpdateQuestion(question);

                var oldAnswers = await _answerService.GetQuestionAnswers(question.QuestionId);

                var removedAnswers = oldAnswers.Data.Where(a => !answers
                .Select(o => o.AnswerId)
                .ToList()
                .Contains(a.AnswerId))
                    .ToList();

                if (removedAnswers.Any())
                {
                    var removeAnswersResult = await _answerService.RemoveRangeOfAnswers(removedAnswers);

                    if (!removeAnswersResult.IsSuccessful)
                    {
                        return new Result<bool>(isSuccessful: false, "Fail to add new answers");
                    }
                }

                var newAnswers = answers.Where(a => a.AnswerId == 0).ToList();

                if (newAnswers.Any())
                {
                    var addAnswersResult = await _answerService.AddRangeOfAnswers(newAnswers);

                    if (!addAnswersResult.IsSuccessful)
                    {
                        return new Result<bool>(isSuccessful: false, "Fail to add new answers");
                    }
                }

                var updateAnswers = answers.Where(a => a.AnswerId != 0 && !removedAnswers.Select(r => r.AnswerId).Contains(a.AnswerId)).ToList();

                var updateResult = await _answerService.UpdateRangeOfAnswers(updateAnswers);

                if (!updateResult.IsSuccessful)
                {
                    return new Result<bool>(isSuccessful: false, "Fail to update answers");
                }

                return new Result<bool>(isSuccessful: true);

            }
            catch (Exception ex)
            {
                return new Result<bool>(isSuccessful: false, "Fail to edit question and answers");
            }
        }
    }
}
