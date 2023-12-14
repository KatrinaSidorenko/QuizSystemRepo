namespace QuizSystem.ViewModels.SharedTestViewModels
{
    public class AgreementSharedTestViewModel
    {
        public int TestId { get; set; }
        public int SharedTestId { get; set; }
        public string Description { get; set; }
        public int AttemptCount { get; set; }
        public DateTime AttemptDuration { get; set; }
        public double PassingScore { get; set; }
    }
}
