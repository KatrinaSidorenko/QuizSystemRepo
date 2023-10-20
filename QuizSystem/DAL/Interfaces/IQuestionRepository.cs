using Core.Models;

namespace DAL.Interfaces
{
    public interface IQuestionRepository
    {
        Task AddQuestion(Question question);
        Task<List<Question>> GetTestGuestions(int testId);
        Task DeleteQuestion(int questionId);
        Task<Question> GetQuestionById(int questionId);
        Task UpdateQuestion(Question question);
    }
}
