﻿using Core.Enums;
using Core.Models;
using Core.Settings;
using DAL.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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

            var sqlExpression = "INSERT INTO SharedTests (test_code, start_date, end_date, description, attempt_count, attempt_duration, status, test_id) " +
                     "VALUES (@TestCode, @StartDate, @EndDate, @Description, @AttemptCount, @AttemptDuration, @Status, @TestId);"+
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
                    sharedTest.Description = (string)reader["description"];
                    sharedTest.AttemptCount = (int)reader["attempt_count"];
                    sharedTest.AttemptDuration = (DateTime)reader["attempt_duration"];
                    sharedTest.Status = (SharedTestStatus)reader["status"];
                    sharedTest.TestId = (int)reader["test_id"];
                    sharedTest.SharedTestId = (int)reader["shared_test_id"];
                    sharedTests.Add(sharedTest);
                }
            }

            return sharedTests;
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
                                   $"test_id={sharedTest.TestId} " +
                                   $"WHERE shared_test_id={sharedTest.SharedTestId}";

            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = await command.ExecuteNonQueryAsync();
            }
        }
    }
}
