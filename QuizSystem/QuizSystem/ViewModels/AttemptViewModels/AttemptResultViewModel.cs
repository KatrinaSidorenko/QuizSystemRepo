namespace QuizSystem.ViewModels.AttemptViewModels
{
    public class AttemptResultViewModel
    {
        public int TestId { get; set; }
        public string Name { get; set; }
        public int AttemptId { get; set; }
        public List<AttemptQuestionViewModel> Questions { get; set; }
    }
}
