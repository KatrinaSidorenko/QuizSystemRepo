using Core.Enums;
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

            var sqlExpression = "INSERT INTO TestResults (question_id, answer_id, attempt_id, entered_value, gained_points)" +
                               "VALUES (@QuestionId, @AnswerId, @AttemptId, @EnteredValue, @GainedPoints);" +
                               "SELECT SCOPE_IDENTITY();";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sqlExpression, connection))
                {
                    command.Parameters.AddWithValue("@QuestionId", testResult.QuestionId);
                    command.Parameters.AddWithValue("@AnswerId", testResult.AnswerId);
                    command.Parameters.AddWithValue("@AttemptId", testResult.AttemptId);
                    command.Parameters.AddWithValue("@EnteredValue", testResult.EnteredValue != null ? testResult.EnteredValue : DBNull.Value);
                    command.Parameters.AddWithValue("@GainedPoints", testResult.GainedPoints);


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
        public async Task<TestResult> GetTestResultByAttemptIdandQuestionId(int attemptId, int questionId, int answerId)
        {
            string sqlExpression = $"SELECT * FROM TestResults WHERE attempt_id = {attemptId} and question_id = {questionId} and answer_id = {answerId}";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            TestResult test = new();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    test.TestResultId = (int)reader["test_result_answer_id"];
                    test.QuestionId = (int)reader["question_id"];
                    test.AnswerId = (int)reader["answer_id"];
                    test.AttemptId = (int)reader["attempt_id"];
                    test.GainedPoints = (double)reader["gained_points"];
                    test.EnteredValue = reader["entered_value"] as string;
                }
            }

            return test;           
        }

        public async Task DeleteTestResultByAttempt(int attemptId)
        {
            string sqlExpression = $"DELETE FROM TestResults WHERE attempt_id={attemptId}";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteTestResultByQuestion(int questionId)
        {
            string sqlExpression = $"DELETE FROM TestResults WHERE question_id={questionId}";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<(double sum, int rightAmount)> GetAttemptPointsData(int attemptId)
        {
            string sqlExpression = $"select sum(gained_points) as total_points, count(*) as right_answers_amount from [TestResults] where gained_points != 0 and attempt_id ={attemptId} group by attempt_id";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                (double, int) result = new();

                while (reader.Read())
                {
                    result.Item1 = (double)reader["total_points"];
                    result.Item2 = (int)reader["right_answers_amount"];
                }

                return result;
            }
        }

        public async Task<int> EnteredRightAnswerAmount(int questionId, int sharedTestId)
        {
            string sqlExpression = $"select count(*) as entered_right_answer\r\nfrom [TestResults]\r\nwhere question_id = {questionId} \r\nand lower(entered_value) like (select lower(answer_description) from [Answers] where question_id = {questionId} and is_right = 1)\r\nand\r\nattempt_id in (select attempt_id from [Attempts] where shared_test_id = {sharedTestId}) ";

            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                int result = 0;

                while (reader.Read())
                {
                    result = (int)reader["entered_right_answer"];
                }

                return result;
            }
        }
    }
}
