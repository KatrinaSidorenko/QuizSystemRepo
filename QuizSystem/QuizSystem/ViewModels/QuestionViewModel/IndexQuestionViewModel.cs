using Core.Enums;
using QuizSystem.ViewModels.AnswerViewModels;

namespace QuizSystem.ViewModels.QuestionViewModel
{
    public class IndexQuestionViewModel
    {
        public int QuestionId { get; set; }
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public int Point { get; set; }
        public int TestId { get; set; }
        public List<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();
    }
}
