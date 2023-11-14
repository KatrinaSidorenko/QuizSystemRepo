using Core.Enums;

namespace Core.DTO
{
    public class AttemptQuestionDocumentDTO
    {
        public int QuestionId { get; set; }
        public string Description { get; set; }
        public QuestionType Type { get; set; }
        public float Point { get; set; }
        public float GetedPoints { get; set; }
        public List<AttemptAnswerDocumentDTO> Answers { get; set; }
    }
}
