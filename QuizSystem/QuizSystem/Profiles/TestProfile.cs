using AutoMapper;
using Core.DTO;
using Core.Models;
using QuizSystem.ViewModels.AttemptViewModels;
using QuizSystem.ViewModels.TakeTestViewModels;
using QuizSystem.ViewModels.TestViewModels;

namespace QuizSystem.Profiles
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<Test, IndexTestViewModel>();
            CreateMap<Test, QuestionTestViewModel>();
            CreateMap<TestViewModel, Test>();
            CreateMap<QuestionTestViewModel, Test>();
            CreateMap<Test, TakeTestViewModel>();
            CreateMap<ResultTestViewModel, AttemptResultDTO>();
            CreateMap<Test, ActivityViewModel>();
            CreateMap<Test, MemberTestViewModel>();
        }
    }
}
