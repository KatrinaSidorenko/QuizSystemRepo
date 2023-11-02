namespace QuizSystem.ViewModels.TakeTestViewModels
{
    public class ResultTestViewModel
    {
        public int TestId { get; set; }
        public int TakedTestUserId { get; set; }
        public int AttemptId { get; set; }
        public List<AnswerTakeTestViewModel> Answers { get; set; }
    }
}
