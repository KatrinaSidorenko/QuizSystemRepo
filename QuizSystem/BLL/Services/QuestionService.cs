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
    }
}
