namespace QuizSystem.ViewModels.AttemptViewModels
{
    public class AttemptSharedTestResultViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public double AverageDuration { get; set; }
        public double AveragePoints { get; set; }
        public int AttemptCount { get; set; }
        public int TestId { get; set; }
        public int SharedTestId { get; set; }
    }
}
