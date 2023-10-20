using Core.Enums;
using Core.Models;
using Core.Settings;
using DAL.Interfaces;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace DAL.Repository
{
    public class TagRepository : BaseRepository, ITagRepository
    {
        public TagRepository(IOptions<ConnectionSettings> options) : base(options)
        {
        }

        public async Task<List<Tag>> GetAllTags()
        {
            string sqlExpression = "SELECT * FROM Tags";

            SqlConnection connection = new SqlConnection(_connectionString);
            List<Tag> tags = new List<Tag>();

            using (connection)
            {
                SqlCommand command = new SqlCommand(sqlExpression, connection);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    Tag tag = new Tag();
                    tag.TagId = (int)reader["tag_id"];
                    tag.Description = (string)reader["tag_description"];                   
                    tags.Add(tag);
                }
            }

            return tags;
        }
    }
}
