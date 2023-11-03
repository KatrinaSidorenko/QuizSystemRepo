using AutoMapper;
using Core.Models;
using QuizSystem.ViewModels.AttemptViewModel;

namespace QuizSystem.Profiles
{
    public class AttemptProfile : Profile
    {
        public AttemptProfile()
        {
            CreateMap<Attempt, AttemptViewModel>();   
        }
    }
}
