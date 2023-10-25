using AutoMapper;
using Core.Models;
using QuizSystem.ViewModels.UserViewModels;

namespace QuizSystem.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, EditUserViewModel>();
            CreateMap<EditUserViewModel, User>();
            CreateMap<CreateUserViewModel, User>();
        }
    }
}
