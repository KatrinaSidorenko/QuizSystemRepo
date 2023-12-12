using Core.DTO;
using Core.Models;

namespace DAL.Interfaces
{
    public interface IQuestionRepository
    {
        Task<int> AddQuestion(Question question);
        Task<List<Question>> GetTestQuestions(int testId);
        Task DeleteQuestion(int questionId);
        Task<Question> GetQuestionById(int questionId);
        Task UpdateQuestion(Question question);
        Task<List<QuestionStatDTO>> GetTestQuestionsDTO(int testId);
        Task<bool> IsInTestQuestions(int testId);
        Task<int> SelectTotalPointsForTest(int testId);
    }
}
