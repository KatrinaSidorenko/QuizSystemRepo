using BLL.Interfaces;
using Core.Models;
using DAL.Interfaces;
using DAL.Repository;

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

            if (answers.Where(a => a.IsRight).Count() == 0)
            {
                return new Result<Question>(false, "Should be one right answer");
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

            if (answers.Where(a => a.IsRight).Count() == 0)
            {
                return new Result<bool>(false, "Should be one right answer");
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

        public async Task<Result<List<Question>>> GetTestQuestions(int testId)
        {
            try
            {
                var questions = await _questionRepository.GetTestQuestions(testId);

                return new Result<List<Question>>(true, questions);
            }
            catch (Exception ex)
            {
                return new Result<List<Question>>(isSuccessful: false, "Fail to get questions");
            }
        }

        public async Task<Result<Dictionary<int, List<int>>>> GetTestQuestionsWithRightAnswers(int testId)
        {
            try
            {
                var dict = new Dictionary<int, List<int>>();
                var testQuestions = await _questionRepository.GetTestQuestions(testId);
                var questionsId = testQuestions.Select(t => t.QuestionId).ToList();

                foreach (var questionId in questionsId)
                {
                    var answers = await _answerService.GetQuestionAnswers(questionId);
                    var rightAnswersId = answers.Data
                        .Where(a => a.IsRight)
                        .Select(a => a.AnswerId)
                        .ToList();

                    dict.Add(questionId, rightAnswersId);
                }

                return new Result<Dictionary<int, List<int>>>(true, dict);
            }
            catch (Exception ex)
            {
                return new Result<Dictionary<int, List<int>>>(isSuccessful: false, "Fail to get questions with answers");
            }
        }
        public async Task<Result<Question>> GetQuestionById(int questionId)
        {
            try
            {
                var question = await _questionRepository.GetQuestionById(questionId);

                if (question == null)
                {
                    return new Result<Question>(isSuccessful: false, "Fail to get question");
                }

                return new Result<Question>(true, question);
            }
            catch (Exception ex)
            {
                return new Result<Question>(isSuccessful: false, "Fail to get question");
            }
        }

    }
}
