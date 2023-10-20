using Core.Enums;
using Core.Models;

namespace QuizSystem.ViewModels.QuestionViewModel
{
    public class CreateQuestionViewModel
    {
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public int Point { get; set; }
        public int TestId { get; set; }
        public List<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();
    }
}
