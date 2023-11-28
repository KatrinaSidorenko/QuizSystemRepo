using AutoMapper;
using Core.DTO;
using Core.Models;
using QuizSystem.ViewModels.QuestionViewModel;
using QuizSystem.ViewModels.SharedTestViewModels;
using QuizSystem.ViewModels.TakeTestViewModels;

namespace QuizSystem.Profiles
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<CreateQuestionViewModel, Question>();
            CreateMap<Question, IndexQuestionViewModel>();
            CreateMap<EditQuestionViewModel, Question> ();
            CreateMap<EditQuestionAnswerViewModel, Question> ();
            CreateMap<Question, EditQuestionAnswerViewModel>();
            CreateMap<Question, TakeTestQuestionViewModel>();
            CreateMap<QuestionStatDTO, QuestionsStatViewModel>();
        }
    }
}
