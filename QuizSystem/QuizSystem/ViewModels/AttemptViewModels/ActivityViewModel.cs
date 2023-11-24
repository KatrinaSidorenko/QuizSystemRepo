namespace QuizSystem.ViewModels.AttemptViewModels
{
    public class ActivityViewModel
    {
        public int TestId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime LastAttemptDate { get; set; }
        public int AttemptsAmount { get; set; }
    }
}
