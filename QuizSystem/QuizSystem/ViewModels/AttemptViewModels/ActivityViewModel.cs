namespace QuizSystem.ViewModels.AttemptViewModels
{
    public class ActivityViewModel
    {
        public int TestId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime DateOfCreation { get; set; }
        public int AmountOfAttempts { get; set; }
    }
}
