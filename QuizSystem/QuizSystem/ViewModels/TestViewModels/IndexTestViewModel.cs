using Core.Enums;

namespace QuizSystem.ViewModels.TestViewModels
{
    public class IndexTestViewModel
    {
        public int TestId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Visibility Visibility { get; set; }
        public int UserId { get; set; }
        public DateTime DateOfCreation { get; set; }
    }
}
