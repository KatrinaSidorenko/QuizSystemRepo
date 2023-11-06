namespace QuizSystem.ViewModels.AttemptViewModel
{
    public class AttemptResultViewModel
    {
        public int TestId { get; set; }
        public string Name { get; set; }
        public List<AttemptQuestionViewModel> Questions { get; set; }
    }
}
