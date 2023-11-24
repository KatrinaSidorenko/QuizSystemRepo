using AutoMapper;
using Core.DTO;
using Core.Models;
using QuizSystem.ViewModels.AttemptViewModels;
using QuizSystem.ViewModels.PaginationTestViewModels;

namespace QuizSystem.Profiles
{
    public class AttemptProfile : Profile
    {
        public AttemptProfile()
        {
            CreateMap<Attempt, AttemptViewModel>();   
            CreateMap<StatisticAttemptsDTO, StatisticViewModel>();
            CreateMap<AttemptResultViewModel, AttemptResultDocumentDTO>();
            CreateMap<AttemptHistoryDTO, AttemptViewModel>();
            CreateMap<SharedAttemptDTO, AttemptSharedTestResultViewModel>();
        }
    }
}
