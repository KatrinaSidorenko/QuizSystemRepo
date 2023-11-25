using System.ComponentModel.DataAnnotations;

namespace QuizSystem.ViewModels.SharedTestViewModels
{
    public class CreateShareTestViewModel
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }

        [Required]
        public int AttemptCount { get; set; }

        [Required]
        public int AttemptDuration { get; set; }
        [Required]
        public string PassScore { get; set; }
    }
}
