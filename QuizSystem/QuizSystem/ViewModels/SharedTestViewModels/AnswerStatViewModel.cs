namespace QuizSystem.ViewModels.SharedTestViewModels
{
    public class AnswerStatViewModel
    {
        public int AnswerId { get; set; }
        public string Value { get; set; }
        public bool IsRight { get; set; }
        public int QuestionId { get; set; }
        public double UserChooseProcent { get; set; }
    }
}
