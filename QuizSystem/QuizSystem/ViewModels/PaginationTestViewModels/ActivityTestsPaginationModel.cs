using Core.Enums;
using QuizSystem.ViewModels.AttemptViewModels;

namespace QuizSystem.ViewModels.PaginationTestViewModels
{
    public class ActivityTestsPaginationModel
    {
        public List<ActivityViewModel> Activities { get; set; }
        public int? CurrentPageIndex { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int UserId { get; set; }
        public string SearchParam { get; set; }
        public Visibility? FilterParam { get; set; }
        public SortingParam SortingParam { get; set; }
    }
}
