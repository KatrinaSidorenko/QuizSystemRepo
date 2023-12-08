using AutoMapper;
using Core.DTO;
using Core.Models;
using QuizSystem.ViewModels.AnswerViewModels;
using QuizSystem.ViewModels.SharedTestViewModels;
using QuizSystem.ViewModels.TakeTestViewModels;

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
            CreateMap<Answer, AnswerTakeTestViewModel>();
            CreateMap<AnswerTakeTestViewModel, AnswerResultDTO>();
            CreateMap<AnswerTakeTestViewModel, Answer>();
            CreateMap<AnswerStatDTO, AnswerStatViewModel>();
        }
    }
}
