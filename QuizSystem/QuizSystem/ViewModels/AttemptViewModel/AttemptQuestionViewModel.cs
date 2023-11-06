using Core.Enums;
using Core.Models;

namespace QuizSystem.ViewModels.AttemptViewModel
{
    public class AttemptQuestionViewModel
    {
        public int QuestionId { get; set; }
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public int Point { get; set; }
        public int GetedPoints { get; set; }
        public List<AttemptAnswerViewModel> Answers { get; set; }
    }
}
