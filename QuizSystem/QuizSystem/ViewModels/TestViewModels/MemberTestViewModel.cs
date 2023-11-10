using Core.Enums;
using QuizSystem.ViewModels.QuestionViewModel;
using System.ComponentModel.DataAnnotations;

namespace QuizSystem.ViewModels.TestViewModels
{
    public class MemberTestViewModel
    {
        public int TestId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Visibility Visibility { get; set; }
        public int UserId { get; set; }
        public DateTime DateOfCreation { get; set; }
        public List<IndexQuestionViewModel> Questions { get; set; } = new List<IndexQuestionViewModel>();
        public int AmountOfQuestions { get; set; }
        public double TotalMark { get; set; }
    }
}
