using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAnswerService
    {
        Task<Result<bool>> AddRangeOfAnswers(List<Answer> answerList);
        Task<Result<List<Answer>>> GetQuestionAnswers(int questionId);
        Task<Result<bool>> RemoveRangeOfAnswers(List<Answer> answerList);
        Task<Result<bool>> UpdateRangeOfAnswers(List<Answer> answers);
        Task<Result<Answer>> GetAnswerById(int answerId);
    }
}
