using Core.Enums;
using Core.Models;
using System.ComponentModel.DataAnnotations;

namespace QuizSystem.ViewModels.TestViewModels
{
    public class QuestionTestViewModel
    {
        public int TestId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string Description { get; set; }
        public Visibility Visibility { get; set; }
        public int UserId { get; set; }
        public DateTime DateOfCreation { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }
}
