using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IQuestionService
    {
        Task<Result<Question>> AddQuestionWithAnswers(Question question, List<Answer> answers);
        Task<Result<bool>> EditQuestionAndAnswers(Question question, List<Answer> answers);
        Task<Result<List<Question>>> GetTestQuestions(int testId);
    }
}
