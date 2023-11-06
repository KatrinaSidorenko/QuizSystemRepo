namespace QuizSystem.ViewModels.AttemptViewModel
{
    public class AttemptAnswerViewModel
    {
        public int AnswerId { get; set; }
        public string Value { get; set; }
        public bool IsRight { get; set; }
        public bool ChoosenByUser { get; set; }
        public string ValueByUser { get; set; }
    }
}
