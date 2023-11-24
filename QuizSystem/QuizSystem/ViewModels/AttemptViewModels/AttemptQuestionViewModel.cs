using Core.Enums;
using Core.Models;

namespace QuizSystem.ViewModels.AttemptViewModels
{
    public class AttemptQuestionViewModel
    {
        public int QuestionId { get; set; }
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public float Point { get; set; }
        public float GetedPoints { get; set; }
        public List<AttemptAnswerViewModel> Answers { get; set; }
    }
}
