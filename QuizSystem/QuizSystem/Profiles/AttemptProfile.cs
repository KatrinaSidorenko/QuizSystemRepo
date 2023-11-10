using AutoMapper;
using Core.DTO;
using Core.Models;
using QuizSystem.ViewModels.AttemptViewModel;

namespace QuizSystem.Profiles
{
    public class AttemptProfile : Profile
    {
        public AttemptProfile()
        {
            CreateMap<Attempt, AttemptViewModel>();   
            CreateMap<StatisticAttemptsDTO, StatisticViewModel>();
        }
    }
}
