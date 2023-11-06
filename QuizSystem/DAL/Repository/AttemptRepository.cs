using Core.Enums;
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

        public async Task<Attempt> GetAttemptById(int attemptId)
        {
            string sqlExpression = $"SELECT * FROM Attempts Where attempt_id = {attemptId}";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            Attempt attempt = new();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    attempt.TestId = (int)reader["test_id"];
                    attempt.Points = (double)reader["points"];
                    attempt.StartDate = (DateTime)reader["start_date"];
                    attempt.EndDate = (DateTime)reader["end_date"];
                    attempt.UserId = (int)reader["user_id"];
                    attempt.RightAnswersAmount = (int)reader["right_answers_amount"];
                    attempt.SharedTestId = (int)reader["shared_test_id"];
                    attempt.AttemptId = (int)reader["attempt_id"];
                }              
            }

            return attempt;
        }

        public Task<List<Attempt>> GetUserAttempts(int userId, int testId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAttempt(Attempt attempt)
        {
            if (attempt == null)
            {
                throw new ArgumentNullException(nameof(attempt));
            }
            string sqlExpression = "UPDATE Attempts SET points = @Points, start_date = @StartDate, end_date = @EndDate, shared_test_id = @SharedTestId, right_answers_amount = @RightAnswersAmount, test_id = @TestId, user_id = @UserId WHERE attempt_id = @AttemptId";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.AddWithValue("@Points", attempt.Points);
                command.Parameters.AddWithValue("@StartDate", attempt.StartDate);
                command.Parameters.AddWithValue("@EndDate", attempt.EndDate);
                command.Parameters.AddWithValue("@SharedTestId", attempt.SharedTestId);
                command.Parameters.AddWithValue("@RightAnswersAmount", attempt.RightAnswersAmount);
                command.Parameters.AddWithValue("@TestId", attempt.TestId);
                command.Parameters.AddWithValue("@UserId", attempt.UserId);
                command.Parameters.AddWithValue("@AttemptId", attempt.AttemptId);
                connection.Open();              
                int number = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<Dictionary<int, int>> GetUserTestAttemptsId(int userId)
        {
            string sqlExpresiion = $"select distinct test_id, count(*) as total from [Attempts] where user_id = {userId} group by test_id";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpresiion, connection);
            Dictionary<int, int> testIds = new();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while(reader.Read())
                {
                    var testId = (int)reader["test_id"];
                    var attemptCount = (int)reader["total"];

                    testIds.Add(testId, attemptCount);
                }

                return testIds;
            }
        }
    }
}
