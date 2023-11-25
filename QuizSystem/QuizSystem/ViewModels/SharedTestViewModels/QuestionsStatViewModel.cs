using Core.Enums;

namespace QuizSystem.ViewModels.SharedTestViewModels
{
    public class QuestionsStatViewModel
    {
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public int Point { get; set; }
        public double GetMaxPointProcent { get; set; }
    }
}
