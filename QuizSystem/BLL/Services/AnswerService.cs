using BLL.Interfaces;
using Core.Models;
using DAL.Interfaces;
using DAL.Repository;


namespace BLL.Services
{
    public class AnswerService :IAnswerService
    {
        private readonly IAnswerRepository _answerRepository;
        public AnswerService(IAnswerRepository answerRepository)
        {
            _answerRepository = answerRepository;
        }

        public async Task<Result<bool>> AddRangeOfAnswers(List<Answer> answerList)
        {
            if (!answerList.Any())
            {
                return new Result<bool>(false);
            }

            try
            {
                var tasks = new List<Task>();  
                
                foreach (var answer in answerList)
                {
                    if (answer.QuestionId != 0)
                    {
                        tasks.Add(_answerRepository.AddAnswer(answer));
                    }
                }

                await Task.WhenAll(tasks);

                return new Result<bool>(true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(false, "Fail to add answers to question");
            }
        }

        public async Task<Result<List<Answer>>> GetQuestionAnswers(int questionId)
        {
            try
            {
                var answers = await _answerRepository.GetQuestionAnswers(questionId);

                return new Result<List<Answer>>(true, answers);
            }
            catch (Exception ex)
            {
                return new Result<List<Answer>>(false, "Fail to get answers to question");
            }
        }

        public async Task<Result<bool>> RemoveRangeOfAnswers(List<Answer> answerList)
        {
            if (!answerList.Any())
            {
                return new Result<bool>(false);
            }

            try
            {
                var tasks = new List<Task>();

                foreach (var answer in answerList)
                {
                    if (answer.QuestionId != 0)
                    {
                        tasks.Add(_answerRepository.DeleteAnswer(answer.AnswerId));
                    }
                }

                await Task.WhenAll(tasks);

                return new Result<bool>(true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(false, "Fail to delete answers to question");
            }
        }

        public async Task<Result<bool>> UpdateRangeOfAnswers(List<Answer> answers)
        {
            if (!answers.Any())
            {
                return new Result<bool>(false);
            }

            try
            {
                var tasks = new List<Task>();

                foreach (var answer in answers)
                {
                    if (answer.QuestionId != 0)
                    {
                        tasks.Add(_answerRepository.UpdateAnswer(answer));
                    }
                }

                await Task.WhenAll(tasks);

                return new Result<bool>(true);
            }
            catch (Exception ex)
            {
                return new Result<bool>(false, "Fail to delete answers to question");
            }
        }
        public async Task<Result<Answer>> GetAnswerById(int answerId)
        {
            try
            {
                var answer = await _answerRepository.GetAnswerById(answerId);

                if (answer == null)
                {
                    return new Result<Answer>(isSuccessful: false, $"Fail to get {nameof(answer)}");
                }

                return new Result<Answer>(true, answer);
            }
            catch (Exception ex)
            {
                return new Result<Answer>(isSuccessful: false, $"Fail to get test");
            }
        }
    }
}
