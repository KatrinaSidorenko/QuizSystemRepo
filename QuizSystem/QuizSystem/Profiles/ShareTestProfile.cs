using AutoMapper;
using Core.DTO;
using Core.Models;
using QuizSystem.ViewModels.SharedTestViewModels;

namespace QuizSystem.Profiles
{
    public class ShareTestProfile : Profile
    {
        public ShareTestProfile()
        {
            CreateMap<CreateShareTestViewModel, SharedTest>();
            CreateMap<SharedTest, DetailsSharedTestViewModel>();
            CreateMap<SharedTestDTO, IndexSharedTestViewModel>();
        }
    }
}
