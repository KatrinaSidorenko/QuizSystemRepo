using Core.Models;
using Core.Settings;
using DAL.Interfaces;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace DAL.Repository
{
    public class TestResultRepository : BaseRepository, ITestResultRepository
    {
        public TestResultRepository(IOptions<ConnectionSettings> options) : base(options)
        {
        }

        public async Task<int> AddTestResult(TestResult testResult)
        {
            if (testResult == null)
            {
                throw new ArgumentNullException(nameof(testResult));
            }

            var sqlExpression = "INSERT INTO TestResults (question_id, answer_id, attempt_id)" +
                               "VALUES (@QuestionId, @AnswerId, @AttemptId);" +
                               "SELECT SCOPE_IDENTITY();";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sqlExpression, connection))
                {
                    command.Parameters.AddWithValue("@QuestionId", testResult.QuestionId);
                    command.Parameters.AddWithValue("@AnswerId", testResult.AnswerId);
                    command.Parameters.AddWithValue("@AttemptId", testResult.AttemptId);

                    var insertedId = await command.ExecuteScalarAsync();

                    if (int.TryParse(insertedId?.ToString(), out int id))
                    {
                        return id;
                    }
                    else
                    {
                        throw new InvalidOperationException("Unable to retrieve the inserted ID.");
                    }
                }
            }
        }

    }
}
