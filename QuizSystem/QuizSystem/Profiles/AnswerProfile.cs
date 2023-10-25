using AutoMapper;
using Core.Models;
using QuizSystem.ViewModels.AnswerViewModels;

namespace QuizSystem.Profiles
{
    public class AnswerProfile : Profile
    {
        public AnswerProfile()
        {
            CreateMap<Answer, EditAnswerViewModel>();
            CreateMap<EditAnswerViewModel, Answer>();
            CreateMap<AnswerViewModel, Answer>();
            CreateMap<Answer, AnswerViewModel>();
        }
    }
}
