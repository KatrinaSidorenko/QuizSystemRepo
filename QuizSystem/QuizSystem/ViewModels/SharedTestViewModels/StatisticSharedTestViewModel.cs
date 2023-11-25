namespace QuizSystem.ViewModels.SharedTestViewModels
{
    public class StatisticSharedTestViewModel
    {
        public int TakenCountByUsers { get; set; }
        public double AvgPoints { get; set; }
        public double PassedUsersProcent { get; set; }
        public List<UserStatViewModel> UsersStat { get; set; }
    }
}
