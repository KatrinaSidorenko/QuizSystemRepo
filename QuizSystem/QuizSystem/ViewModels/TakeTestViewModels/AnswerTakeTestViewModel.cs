namespace QuizSystem.ViewModels.TakeTestViewModels
{
    public class AnswerTakeTestViewModel
    {
        public int AnswerId { get; set; }
        public string Value { get; set; }
        public bool IsRight { get; set; }
        public int QuestionId { get; set; }
    }
}
