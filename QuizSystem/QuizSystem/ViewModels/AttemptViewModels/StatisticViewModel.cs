using QuizSystem.ViewModels.SharedTestViewModels;
namespace QuizSystem.ViewModels.AttemptViewModels
{
    public class StatisticViewModel
    {
        public int AmountOfAttempts { get; set; }
        public double AverageTime { get; set; }
        public double AverageMark { get; set; }
        public DateTime FirstAttemptDate { get; set; }
        public DateTime LastAttemptDate { get; set; }
        public double FirstMarkResult { get; set; }
        public double LastMarkResult { get; set; }
        public double Progress { get; set; }
        public List<QuestionsStatViewModel> QuestionsStat { get; set; }
    }
}
