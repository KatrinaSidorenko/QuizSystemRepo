﻿using Core.Enums;
using QuizSystem.ViewModels.TestViewModels;

namespace QuizSystem.ViewModels.PaginationTestViewModels
{
    public class TestPaginationModel
    {
        public List<IndexTestViewModel> Tests { get; set; } = new();
        public int? CurrentPageIndex { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public string SearchParam { get; set; } 
        public SortingParam SortingParam { get; set; }
    }
}
