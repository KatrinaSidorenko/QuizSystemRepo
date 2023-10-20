using Core.Models;
using Core.Enums;
using Core.Settings;
using DAL.Interfaces;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace DAL.Repository
{
    public class TestRepository : BaseRepository, ITestRepository
    {
        public TestRepository(IOptions<ConnectionSettings> options) : base(options)
        {
        }

        public async Task<List<Test>> GetAllTests()
        {
            string sqlExpression = "SELECT * FROM Tests";

            SqlConnection connection = new SqlConnection(_connectionString);
            List<Test> tests = new List<Test>();

            using (connection)
            {
                SqlCommand command = new SqlCommand(sqlExpression, connection);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    Test test = new Test();
                    test.TestId = (int)reader["test_id"];
                    test.Name = (string)reader["test_name"];
                    test.Description = (string)reader["test_description"];
                    test.Visibility = (Visibility)reader["test_visibility"];
                    test.DateOfCreation = (DateTime)reader["date_of_creation"];
                    test.UserId = (int)reader["user_id"];
                    tests.Add(test);
                }
            }

            return tests;
        }

        //public async Task AddTest(Test test)
        //{
        //    if (test == null)
        //    {
        //        throw new ArgumentNullException(nameof(test));
        //    }

        //    var sqlExpression = $"INSERT INTO Tests (test_name, test_description, test_visibility, date_of_creation, user_id) VALUES ('{test.Name}', '{test.Description}', '{(int)test.Visibility}', '{test.DateOfCreation}', '{test.UserId}')";
        //    SqlConnection connection = new SqlConnection(_connectionString);

        //    using (connection)
        //    {
        //        connection.Open();
        //        SqlCommand command = new SqlCommand(sqlExpression, connection);
        //        int number = await command.ExecuteNonQueryAsync();
        //    }
        //}
        public async Task<int> AddTest(Test test)
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            var sqlExpression = $"INSERT INTO Tests (test_name, test_description, test_visibility, date_of_creation, user_id) " +
                               $"VALUES ('{test.Name}', '{test.Description}', '{(int)test.Visibility}', '{test.DateOfCreation.ToString("yyyy-MM-ddTHH:mm:ss")}', '{test.UserId}');" +
                               "SELECT SCOPE_IDENTITY();"; // This line retrieves the last inserted ID

            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);

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

        public async Task<Test> GetTestById(int id)
        {
            string sqlExpression = $"SELECT * FROM Tests Where test_id = '{id}'";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            Test test = new ();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    test.TestId = (int)reader["test_id"];
                    test.Name = (string)reader["test_name"];
                    test.Description = (string)reader["test_description"];
                    test.Visibility = (Visibility)reader["test_visibility"];
                    test.DateOfCreation = (DateTime)reader["date_of_creation"];
                    test.UserId = (int)reader["user_id"];
                }
            }

            return test;
        }

        public async Task UpdateTest(Test test)
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            string sqlExpression = $"UPDATE Tests SET test_name='{test.Name}', test_description='{test.Description}', test_visibility={(int)test.Visibility}, date_of_creation='{test.DateOfCreation.ToString("yyyy-MM-ddTHH:mm:ss")}', user_id={test.UserId} WHERE test_id={test.TestId}";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteTest(int testId)
        {
            string sqlExpression = $"DELETE FROM Tests WHERE test_id={testId}";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<Test>> GetUserTests(int userId)
        {
            string sqlExpression = $"SELECT * FROM Tests Where user_id = '{userId}'";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            List<Test> tests = new List<Test>();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    Test test = new Test();
                    test.TestId = (int)reader["test_id"];
                    test.Name = (string)reader["test_name"];
                    test.Description = (string)reader["test_description"];
                    test.Visibility = (Visibility)reader["test_visibility"];
                    test.DateOfCreation = (DateTime)reader["date_of_creation"];
                    test.UserId = (int)reader["user_id"];
                    tests.Add(test);
                }
            }

            return tests;
        }
    }
}
