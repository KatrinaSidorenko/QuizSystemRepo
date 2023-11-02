using Core.Enums;

namespace QuizSystem.ViewModels.TakeTestViewModels
{
    public class TakeTestQuestionViewModel
    {
        public int QuestionId { get; set; }
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public int Point { get; set; }
        public List<AnswerTakeTestViewModel> QuestionAnswers { get; set; }
    }
}
