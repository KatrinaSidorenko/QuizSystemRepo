using Core.Enums;
using QuizSystem.ViewModels.AttemptViewModels;

namespace QuizSystem.ViewModels.PaginationTestViewModels
{
    public class AttempyHistoryPaginationModel
    {
        public int UserId { get; set; }
        public int TestId { get; set; }
        public int? SharedTestId { get; set; }
        public int? CurrentPageIndex { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public string SearchParam { get; set; }
        public SortingParam SortingParam { get; set; }
        public FilterParam FilterParam { get; set; }
        public List<AttemptViewModel> Attempts { get; set; }
    }
}
