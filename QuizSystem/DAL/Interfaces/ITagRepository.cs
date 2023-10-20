using Core.Models;


namespace DAL.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllTags();
    }
}
