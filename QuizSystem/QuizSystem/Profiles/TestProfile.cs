using AutoMapper;
using Core.Models;
using QuizSystem.ViewModels.TestViewModels;

namespace QuizSystem.Profiles
{
    public class TestProfile : Profile
    {
        public TestProfile()
        {
            CreateMap<Test, IndexTestViewModel>();
            CreateMap<Test, QuestionTestViewModel>();
        }
    }
}
