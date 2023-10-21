namespace QuizSystem.ViewModels.AnswerViewModels
{
    public class EditAnswerViewModel
    {
        public int AnswerId { get; set; }
        public string Value { get; set; }
        public bool IsRight { get; set; }
        public int QuestionId { get; set; }
    }
}
