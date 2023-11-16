using Core.Enums;
using QuizSystem.ViewModels.SharedTestViewModels;

namespace QuizSystem.ViewModels.PaginationTestViewModels
{
    public class SharedTestPaginationModel
    {
        public int? CurrentPageIndex { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public string SearchParam { get; set; }
        public SharedTestStatus? FilterParam { get; set; }
        public SortingParam SortingParam { get; set; }
        public List<IndexSharedTestViewModel> Tests { get; set; }
    }
}
