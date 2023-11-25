using Core.DTO;
using Core.Enums;
using Core.Models;
using Core.Settings;
using DAL.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class SharedTestRepository : BaseRepository, ISharedTestRepository
    {
        public SharedTestRepository(IOptions<ConnectionSettings> options) : base(options)
        {
        }

        public async Task<int> AddSharedTest(SharedTest sharedTest)
        {
            if (sharedTest == null)
            {
                throw new ArgumentNullException(nameof(sharedTest));
            }

            var sqlExpression = "INSERT INTO SharedTests (test_code, start_date, end_date, description, attempt_count, attempt_duration, status, test_id, passing_score) " +
                     "VALUES (@TestCode, @StartDate, @EndDate, @Description, @AttemptCount, @AttemptDuration, @Status, @TestId, @PassingScore);"+
                               "SELECT SCOPE_IDENTITY();";


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.Parameters.AddWithValue("@TestCode", sharedTest.TestCode);
                command.Parameters.AddWithValue("@StartDate", sharedTest.StartDate);
                command.Parameters.AddWithValue("@EndDate", sharedTest.EndDate);
                command.Parameters.AddWithValue("@Description", sharedTest.Description != null ? sharedTest.Description : DBNull.Value);
                command.Parameters.AddWithValue("@AttemptCount", sharedTest.AttemptCount);
                command.Parameters.AddWithValue("@AttemptDuration", sharedTest.AttemptDuration);
                command.Parameters.AddWithValue("@Status", (int)sharedTest.Status);
                command.Parameters.AddWithValue("@TestId", sharedTest.TestId);
                command.Parameters.AddWithValue("@PassingScore", sharedTest.PassingScore);


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

        public async Task<SharedTest> GetSharedTestById(int sharedTestId)
        {
            string sqlExpression = $"SELECT * FROM SharedTests WHERE shared_test_id = {sharedTestId}";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SharedTest sharedTest = new SharedTest();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    sharedTest.TestCode = (Guid)reader["test_code"];
                    sharedTest.StartDate = (DateTime)reader["start_date"];
                    sharedTest.EndDate = (DateTime)reader["end_date"];
                    sharedTest.Description = reader["description"] as string;
                    sharedTest.AttemptCount = (int)reader["attempt_count"];
                    sharedTest.AttemptDuration = (DateTime)reader["attempt_duration"];
                    sharedTest.Status = (SharedTestStatus)reader["status"];
                    sharedTest.TestId = (int)reader["test_id"];
                    sharedTest.SharedTestId = (int)reader["shared_test_id"];
                    sharedTest.PassingScore = reader["passing_score"].Equals(DBNull.Value) ? 0 : (double)Convert.ChangeType(reader["passing_score"], typeof(double));
                }
            }

            return sharedTest;
        }
        public async Task<SharedTest> GetSharedTestByCode(Guid code)
        {
            string sqlExpression = $"SELECT * FROM SharedTests WHERE test_code = '{code}'";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SharedTest sharedTest = new SharedTest();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    sharedTest.TestCode = (Guid)reader["test_code"];
                    sharedTest.StartDate = (DateTime)reader["start_date"];
                    sharedTest.EndDate = (DateTime)reader["end_date"];
                    sharedTest.Description = reader["description"] as string;
                    sharedTest.AttemptCount = (int)reader["attempt_count"];
                    sharedTest.AttemptDuration = (DateTime)reader["attempt_duration"];
                    sharedTest.Status = (SharedTestStatus)reader["status"];
                    sharedTest.TestId = (int)reader["test_id"];
                    sharedTest.SharedTestId = (int)reader["shared_test_id"];
                    sharedTest.PassingScore = reader["passing_score"].Equals(DBNull.Value) ? 0 : (double)Convert.ChangeType(reader["passing_score"], typeof(double));
                }
            }

            return sharedTest;
        }


        public async Task<List<SharedTest>> GetTests()
        {
            string sqlExpression = $"SELECT * FROM SharedTests";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            List<SharedTest> sharedTests = new List<SharedTest>();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    SharedTest sharedTest = new SharedTest();
                    sharedTest.TestCode = (Guid)reader["test_code"];
                    sharedTest.StartDate = (DateTime)reader["start_date"];
                    sharedTest.EndDate = (DateTime)reader["end_date"];
                    sharedTest.Description = reader["description"] as string;
                    sharedTest.AttemptCount = (int)reader["attempt_count"];
                    sharedTest.AttemptDuration = (DateTime)reader["attempt_duration"];
                    sharedTest.Status = (SharedTestStatus)reader["status"];
                    sharedTest.TestId = (int)reader["test_id"];
                    sharedTest.SharedTestId = (int)reader["shared_test_id"];
                    sharedTest.PassingScore = reader["passing_score"].Equals(DBNull.Value) ? 0 : (double)Convert.ChangeType(reader["passing_score"], typeof(double));

                    sharedTests.Add(sharedTest);
                }
            }

            return sharedTests;
        }

        public async Task<List<SharedTestDTO>> GetUserSharedTests(int userId)
        {
            string sqlExpression = $"select *, (select test_name from [Tests] where test_id = [SharedTests].test_id) as test_name from [SharedTests] where test_id in (select test_id from [Tests] where user_id = {userId})";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            List<SharedTestDTO> sharedTests = new();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    SharedTestDTO sharedTest = new();
                    sharedTest.TestCode = (Guid)reader["test_code"];
                    sharedTest.StartDate = (DateTime)reader["start_date"];
                    sharedTest.EndDate = (DateTime)reader["end_date"];
                    sharedTest.Description = reader["description"] as string;
                    sharedTest.AttemptCount = (int)reader["attempt_count"];
                    sharedTest.AttemptDuration = (DateTime)reader["attempt_duration"];
                    sharedTest.Status = (SharedTestStatus)reader["status"];
                    sharedTest.TestId = (int)reader["test_id"];
                    sharedTest.SharedTestId = (int)reader["shared_test_id"];
                    sharedTest.TestName = (string)reader["test_name"];
                    sharedTest.PassingScore = reader["passing_score"].Equals(DBNull.Value) ? 0 : (double)Convert.ChangeType(reader["passing_score"], typeof(double));

                    sharedTests.Add(sharedTest);
                }

                return sharedTests;
            }
        }

        public async Task DeleteSharedTest(int sharedTestId)
        {
            string sqlExpression = $"DELETE FROM SharedTests WHERE shared_test_id={sharedTestId}";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateSharedTest(SharedTest sharedTest)
        {
            if (sharedTest == null)
            {
                throw new ArgumentNullException(nameof(sharedTest));
            }

            string sqlExpression = $"UPDATE SharedTests SET " +
                                   $"test_code='{sharedTest.TestCode}', " +
                                   $"start_date='{sharedTest.StartDate.ToString("yyyy-MM-ddTHH:mm:ss")}', " +
                                   $"end_date='{sharedTest.EndDate.ToString("yyyy-MM-ddTHH:mm:ss")}', " +
                                   $"description='{sharedTest.Description}', " +
                                   $"attempt_count={sharedTest.AttemptCount}, " +
                                   $"attempt_duration='{sharedTest.AttemptDuration}', " +
                                   $"status={(int)sharedTest.Status}, " +
                                   $"test_id={sharedTest.TestId}," +
                                   $"passing_score={sharedTest.PassingScore}" +
                                   $"WHERE shared_test_id={sharedTest.SharedTestId}";

            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> IsTestShared(int testId)
        {
            string sqlExpression = $"SELECT CASE WHEN EXISTS ( SELECT * FROM [SharedTests] WHERE test_id = {testId} and status != 2) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END";
            SqlConnection sqlConnection = new SqlConnection(_connectionString);

            using (sqlConnection)
            {
                sqlConnection.Open();
                SqlCommand command = new(sqlExpression, sqlConnection);
                var result = await command.ExecuteScalarAsync();

                return (bool)result;
            }
        }

        public async Task<bool> IsCodeExist(Guid code)
        {
            string sqlExpression = $"SELECT CASE WHEN EXISTS ( SELECT * FROM [SharedTests] WHERE test_code = '{code}') THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END";
            SqlConnection sqlConnection = new SqlConnection(_connectionString);

            using (sqlConnection)
            {
                sqlConnection.Open();
                SqlCommand command = new(sqlExpression, sqlConnection);
                var result = await command.ExecuteScalarAsync();

                return (bool)result;
            }
        }
        public async Task<(List<SharedTestDTO>, int)> GetUserSharedTestsWithTotalRecords(int userId, int pageNumber = 1, int pageSize = 6, string orderByProp = "shared_test_id", string sortOrder = "asc", SharedTestStatus? filterParam = null)
        {
            string sqlExpression = "PagingUserSharedTests"; // The stored procedure name

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                List<SharedTestDTO> sharedTests = new();
                int totalRecords = 0;
                bool IsNextPageAvailable;

                using (SqlCommand command = new SqlCommand(sqlExpression, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Define the input parameters
                    command.Parameters.AddWithValue("@PageNumber", pageNumber);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    command.Parameters.AddWithValue("@OrderBy", orderByProp);
                    command.Parameters.AddWithValue("@SortOrder", sortOrder);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@FilterParameter", filterParam is null ? DBNull.Value : filterParam);

                    // Define the output parameter for total records
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
                        SharedTestDTO sharedTest = new();
                        sharedTest.TestCode = (Guid)reader["test_code"];
                        sharedTest.StartDate = (DateTime)reader["start_date"];
                        sharedTest.EndDate = (DateTime)reader["end_date"];
                        sharedTest.Description = reader["description"] as string;
                        sharedTest.AttemptCount = (int)reader["attempt_count"];
                        sharedTest.AttemptDuration = (DateTime)reader["attempt_duration"];
                        sharedTest.Status = (SharedTestStatus)reader["status"];
                        sharedTest.TestId = (int)reader["test_id"];
                        sharedTest.SharedTestId = (int)reader["shared_test_id"];
                        sharedTest.TestName = (string)reader["test_name"];
                        sharedTest.PassingScore = reader["passing_score"].Equals(DBNull.Value) ? 0 : (double)Convert.ChangeType(reader["passing_score"], typeof(double));

                        sharedTests.Add(sharedTest);
                    }
                }

                return (sharedTests, totalRecords);
            }
        }

    }
}
