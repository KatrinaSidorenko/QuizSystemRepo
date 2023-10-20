using BLL.Interfaces;
using Core.Models;
using DAL.Interfaces;


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

            if(answerList.Count(a => a.IsRight) == 0)
            {
                return new Result<bool>(false, "Should be one right answer");
            }

            try
            {
                var tasks = new List<Task>();  
                
                foreach (var answer in answerList)
                {
                    if (answer.QuestionId == 0)
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
    }
}
