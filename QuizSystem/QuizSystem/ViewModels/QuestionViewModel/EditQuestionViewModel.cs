using Core.Enums;

namespace QuizSystem.ViewModels.QuestionViewModel
{
    public class EditQuestionViewModel
    {
        public int QuestionId { get; set; }
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public int Point { get; set; }
        public int TestId { get; set; }
    }
}
