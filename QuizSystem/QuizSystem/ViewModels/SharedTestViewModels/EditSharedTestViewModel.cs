using Core.Enums;

namespace QuizSystem.ViewModels.SharedTestViewModels
{
    public class EditSharedTestViewModel
    {
        public int SharedTestId { get; set; }
        public Guid TestCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int AttemptCount { get; set; }
        public int NewAttemptDuration { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public SharedTestStatus Status { get; set; }
        public string PassScore { get; set; }
    }
}
