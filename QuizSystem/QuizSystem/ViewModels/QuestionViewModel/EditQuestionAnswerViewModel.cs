using Core.Enums;
using QuizSystem.ViewModels.AnswerViewModels;
using System.ComponentModel.DataAnnotations;

namespace QuizSystem.ViewModels.QuestionViewModel
{
    public class EditQuestionAnswerViewModel
    {
        public int QuestionId { get; set; }
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public int Point { get; set; }
        public int TestId { get; set; }
        public List<EditAnswerViewModel> Answers { get; set; } = new List<EditAnswerViewModel>();
    }
}
