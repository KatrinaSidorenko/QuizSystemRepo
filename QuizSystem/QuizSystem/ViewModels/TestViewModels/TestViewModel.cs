using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace QuizSystem.ViewModels.TestViewModels
{
    public class TestViewModel
    {
        public int TestId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string Description { get; set; }
        public Visibility Visibility { get; set; }
        public int UserId { get; set; }
        public DateTime DateOfCreation { get; set; }
    }
}
