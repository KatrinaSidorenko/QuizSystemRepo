using Core.Models;
using Core.Settings;
using DAL.Interfaces;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace DAL.Repository
{
    public class AttemptRepository : BaseRepository, IAttemptRepository
    {
        public AttemptRepository(IOptions<ConnectionSettings> options) : base(options)
        {
        }

        public async Task<int> AddAttempt(Attempt attempt)
        {
            if (attempt == null)
            {
                throw new ArgumentNullException(nameof(attempt));
            }

            var sqlExpression = "INSERT INTO Attempts (points, start_date, end_date, shared_test_id, right_answers_amount, test_id, user_id)" +
                   "VALUES (@Points, @StartDate, @EndDate, @SharedTestId, @RightAnswersAmount, @TestId, @UserId);" +
                   "SELECT SCOPE_IDENTITY();";

            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.AddWithValue("@Points", attempt.Points);
                command.Parameters.AddWithValue("@StartDate", attempt.StartDate);
                command.Parameters.AddWithValue("@EndDate", attempt.EndDate);
                command.Parameters.AddWithValue("@SharedTestId", attempt.SharedTestId);
                command.Parameters.AddWithValue("@RightAnswersAmount", attempt.RightAnswersAmount);
                command.Parameters.AddWithValue("@TestId", attempt.TestId);
                command.Parameters.AddWithValue("@UserId", attempt.UserId);

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

        public Task<Attempt> GetAttemptById(int attemptId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Attempt>> GetUserAttempts(int userId, int testId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAttempt(Attempt attempt)
        {
            throw new NotImplementedException();
        }
    }
}
