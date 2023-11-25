using Core.Enums;

namespace QuizSystem.ViewModels.SharedTestViewModels
{
    public class DetailsSharedTestViewModel
    {
        public int SharedTestId { get; set; }
        public Guid TestCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int AttemptCount { get; set; }
        public DateTime AttemptDuration { get; set; }
        public int TestId { get; set; }
        public double PassingScore { get; set; }
        public string TestName { get; set; }
        public SharedTestStatus Status { get; set; }
    }
}
