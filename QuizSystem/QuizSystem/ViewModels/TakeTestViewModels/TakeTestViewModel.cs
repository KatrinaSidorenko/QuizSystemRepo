namespace QuizSystem.ViewModels.TakeTestViewModels
{
    public class TakeTestViewModel
    {
        public int TestId { get; set; }
        public int AttemptId { get; set; }
        public int? SharedTestId { get; set; }
        public string Name { get; set; }
        public int TakedTestUserId { get; set; }
        public List<TakeTestQuestionViewModel> TakeTestQuestions { get; set;}
    }
}
