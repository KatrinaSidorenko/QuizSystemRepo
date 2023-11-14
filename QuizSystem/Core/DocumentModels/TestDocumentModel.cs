
namespace Core.DocumentModels
{
    public class TestDocumentModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int QuestionsAmount { get; set; }
        public double MaxMark { get; set; }
        public List<QuestionDocumentModel> Questions { get; set; }
    }
}
