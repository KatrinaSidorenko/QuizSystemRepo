using Core.Enums;
using Core.Models;

namespace Core.DocumentModels
{
    public class QuestionDocumentModel
    {
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public int Point { get; set; }
        public List<AnswerDocumentModel> Answers { get; set; }
    }
}
