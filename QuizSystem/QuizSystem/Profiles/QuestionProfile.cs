using AutoMapper;
using Core.Models;
using QuizSystem.ViewModels.QuestionViewModel;

namespace QuizSystem.Profiles
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<CreateQuestionViewModel, Question>();
            CreateMap<Question, IndexQuestionViewModel>();
            CreateMap<EditQuestionViewModel, Question> ();
            CreateMap<Question, EditQuestionAnswerViewModel>();
        }
    }
}
