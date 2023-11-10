using System.ComponentModel.DataAnnotations;

namespace QuizSystem.ViewModels.SharedTestViewModels
{
    public class CreateShareTestViewModel
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int AttemptsCount { get; set; }
        public int AttemptDuration { get; set; }
    }
}
