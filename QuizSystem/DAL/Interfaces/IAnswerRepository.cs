using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IAnswerRepository
    {
        Task AddAnswer(Answer answer);
        Task UpdateAnswer(Answer answer);
        Task DeleteAnswer(int answerId);
        Task<Answer> GetAnswerById(int answerId);
        Task<List<Answer>> GetQuestionAnswers(int questionId);
    }
}
