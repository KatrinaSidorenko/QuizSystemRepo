using Core.DTO;
using Core.Enums;
using Core.Models;
using Core.Settings;
using DAL.Interfaces;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Data;
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
                command.Parameters.AddWithValue("@SharedTestId", attempt.SharedTestId != 0 ? attempt.SharedTestId : DBNull.Value);
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
                    attempt.SharedTestId = reader["shared_test_id"].Equals(DBNull.Value) ? 0 : (int)Convert.ChangeType(reader["shared_test_id"], typeof(int));
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
                command.Parameters.AddWithValue("@SharedTestId", attempt.SharedTestId != 0 ? attempt.SharedTestId : DBNull.Value);
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

        public async Task<List<int>> GetAttemptIdByTest(int testId)
        {
            string sqlExpresiion = $"select attempt_id from Attempts where test_id={testId}";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpresiion, connection);
            List<int> ids = new();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    ids.Add((int)reader["attempt_id"]);
                }

                return ids;
            }
        }

        public async Task<List<int>> GetAttemptIdBySharedTest(int sharedTestId)
        {
            string sqlExpresiion = $"select attempt_id from Attempts where shared_test_id={sharedTestId}";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpresiion, connection);
            List<int> ids = new();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    ids.Add((int)reader["attempt_id"]);
                }

                return ids;
            }
        }
        public async Task<(List<Attempt>, int)> GetAttempts(int testId, int userId, int pageNumber = 1, int pageSize = 6, string orderByProp = "attempt_id", 
            string sortOrder = "asc", int? sharedTestId = null, int startAccuracy = 0, int endAccuracy = 100,
            DateTime? startDate = null, DateTime? endDate = null)
        {           
            string sqlExpression = "PagingAttempts"; 

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                List<Attempt> attempts = new();
                int totalRecords = 0;
                bool IsNextPageAvailable;

                using (SqlCommand command = new SqlCommand(sqlExpression, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@PageNumber", pageNumber);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    command.Parameters.AddWithValue("@OrderBy", orderByProp);
                    command.Parameters.AddWithValue("@SortOrder", sortOrder);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@TestId", testId);
                    command.Parameters.AddWithValue("@SharedTestId", sharedTestId != null ? sharedTestId: DBNull.Value);
                    command.Parameters.AddWithValue("@StartAccuracy", startAccuracy);
                    command.Parameters.AddWithValue("@EndAccuracy", endAccuracy);
                    command.Parameters.AddWithValue("@StartDate", startDate != null ? startDate : DBNull.Value);
                    command.Parameters.AddWithValue("@EndDate", endDate != null ? endDate : DBNull.Value);


                    SqlParameter totalRecordsParam = new SqlParameter("@TotalRecords", SqlDbType.Int);
                    totalRecordsParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(totalRecordsParam);

                    connection.Open();

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        totalRecords = (int)reader["TotalRecords"];
                    }
                    reader.NextResult();
                    var columns = reader.GetColumnSchema();

                    while (reader.Read())
                    {
                        var attempt = new Attempt();
                        attempt.TestId = (int)reader["test_id"];
                        attempt.Points = (double)reader["points"];
                        attempt.StartDate = (DateTime)reader["start_date"];
                        attempt.EndDate = (DateTime)reader["end_date"];
                        attempt.UserId = (int)reader["user_id"];
                        attempt.RightAnswersAmount = (int)reader["right_answers_amount"];
                        attempt.SharedTestId = reader["shared_test_id"].Equals(DBNull.Value) ? 0 : (int)Convert.ChangeType(reader["shared_test_id"], typeof(int));
                        attempt.AttemptId = (int)reader["attempt_id"];
                        attempt.Accuracy = (double)reader["accuracy"];

                        attempts.Add(attempt);
                    }
                }

                return (attempts, totalRecords);
            }
        }

        public async Task<(List<SharedAttemptDTO>, int)> GetSharedAttempts(int sharedTestId, int pageNumber = 1, int pageSize = 6, string orderByProp = "shared_test_id", string sortOrder = "asc")
        {
            string sqlExpression = "PagingUserSharedAttempts"; 

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                List<SharedAttemptDTO> attempts = new();
                int totalRecords = 0;
                bool IsNextPageAvailable;

                using (SqlCommand command = new SqlCommand(sqlExpression, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@PageNumber", pageNumber);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    command.Parameters.AddWithValue("@OrderBy", orderByProp);
                    command.Parameters.AddWithValue("@SortOrder", sortOrder);
                    command.Parameters.AddWithValue("@SharedTestId", sharedTestId);
                    

                    SqlParameter totalRecordsParam = new SqlParameter("@TotalRecords", SqlDbType.Int);
                    totalRecordsParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(totalRecordsParam);

                    connection.Open();

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        totalRecords = (int)reader["TotalRecords"];
                    }
                    reader.NextResult();
                    var columns = reader.GetColumnSchema();

                    while (reader.Read())
                    {
                        var attempt = new SharedAttemptDTO();
                        attempt.TestId = (int)reader["test_id"];
                        attempt.UserId = (int)reader["user_id"];
                        attempt.FirstName = (string)reader["first_name"];
                        attempt.LastName = (string)reader["last_name"];
                        attempt.Email = (string)reader["email"];
                        attempt.SharedTestId = reader["shared_test_id"].Equals(DBNull.Value) ? 0 : (int)Convert.ChangeType(reader["shared_test_id"], typeof(int));
                        attempt.AveragePoints = (double)reader["avg_points"];
                        attempt.AverageDuration = (double)reader["avg_time"];
                        attempt.AttemptCount = (int)reader["attempt_count"];
                       
                        attempts.Add(attempt);
                    }
                }

                return (attempts, totalRecords);
            }
        }

        public async Task<StatisticAttemptsDTO> GetAttemptsStatistic(int testId, int userId)
        {
            string sqlExpression = $"SELECT user_id, test_id, \r\nCOUNT(*) AS entry_count, \r\nAVG(CAST(DATEDIFF(MINUTE, start_date, end_date) AS FLOAT)) AS avg_time, \r\navg(points) as avg_points, max(end_date) as last_attempt,\r\nmin(end_date) as first_attempt\r\nFROM [Attempts] \r\nwhere user_id ={userId} and test_id = {testId} \r\nGROUP BY user_id, test_id;";

            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);

            StatisticAttemptsDTO statistic = new();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    statistic.AmountOfAttempts = (int)reader["entry_count"];
                    statistic.AverageMark = (double)reader["avg_points"];
                    statistic.AverageTime = (double)reader["avg_time"];
                    statistic.FirstAttemptDate = (DateTime)reader["first_attempt"];
                    statistic.LastAttemptDate = (DateTime)reader["last_attempt"];
                }
            }

            return statistic;

        }

        public async Task<double> GetAttemptAccuracy(int attemptId)
        {
            string sqlExpression = $"select (points / \r\n(select sum(point) from Questions Q\r\nwhere A.test_id = Q.test_id\r\ngroup by test_id))*100\r\nfrom [Attempts] A\r\nwhere A.attempt_id = {attemptId}";

            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);

            using(connection)
            {
                connection.Open();
                var result = await command.ExecuteScalarAsync();

                return (double)result;
            }   
        }
        public async Task<int> UserAttemptsCount(int sharedTestId, int userId)
        {
            string sqlExpression = $"select count(*) from [Attempts] \r\nwhere shared_test_id = {sharedTestId} and user_id = {userId}\r\ngroup by shared_test_id;";

            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);

            using (connection)
            {
                connection.Open();
                var result = await command.ExecuteScalarAsync();

                if(result is null)
                {
                    return 0;
                }
                else
                {
                    return (int)result;
                }
            }
        }

        public async Task DeleteAttemptsByTest(int testId)
        {
            string sqlExpression = $"DELETE FROM Attempts WHERE test_id={testId}";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAttemptsBySharedTest(int sharedTestId)
        {
            string sqlExpression = $"DELETE FROM Attempts WHERE shared_test_id={sharedTestId}";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> TotalAmountOfAttemptsBySharedId(int sharedTestId)
        {
            string sqlExpression = $"select count(*) as amount from Attempts where shared_test_id = {sharedTestId}";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);

            using (connection)
            {
                connection.Open();
                var result = await command.ExecuteScalarAsync();

                if (result is null)
                {
                    return 0;
                }
                else
                {
                    return (int)result;
                }
            }
        }

        public async Task<List<(double points, DateTime date)>> GetMaxAndMinAttemptValues(int userId, int testId)
        {
            string sqlExpression = $"SELECT points, end_date\r\nFROM [Attempts]\r\nWHERE test_id = {testId} AND user_id = {userId}\r\n  AND end_date = (SELECT MIN(end_date) FROM [Attempts] WHERE test_id = {testId} AND user_id = {userId})\r\n   OR end_date = (SELECT MAX(end_date) FROM [Attempts] WHERE test_id = {testId} AND user_id = {userId})\r\nORDER BY end_date DESC;";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);

            List<(double points, DateTime date)> date = new();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    (double, DateTime) result = new();
                    result.Item1 = (double)reader["points"];
                    result.Item2 = (DateTime)reader["end_date"];
                    date.Add(result);
                }
            }

            return date;
        }

    }
}
