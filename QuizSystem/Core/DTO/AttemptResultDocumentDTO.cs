
namespace Core.DTO
{
    public class AttemptResultDocumentDTO
    {
        public int TestId { get; set; }
        public string Name { get; set; }
        public double Mark { get; set; }
        public int AttemptId { get; set; }
        public List<AttemptQuestionDocumentDTO> Questions { get; set; }
    }
}
