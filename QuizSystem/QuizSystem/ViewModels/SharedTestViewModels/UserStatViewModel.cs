namespace QuizSystem.ViewModels.SharedTestViewModels
{
    public class UserStatViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public double BestResult { get; set; }
        public double WorstResult { get; set; }
        public double AvgResult { get; set; }
        public int AttemptsAmount { get; set; }
    }
}
