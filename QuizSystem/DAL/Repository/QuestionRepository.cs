using Core.DTO;
using Core.Enums;
using Core.Models;
using Core.Settings;
using DAL.Interfaces;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using static System.Collections.Specialized.BitVector32;

namespace DAL.Repository
{
    public class QuestionRepository : BaseRepository, IQuestionRepository
    {
        public QuestionRepository(IOptions<ConnectionSettings> options) : base(options)
        {
        }

        public async Task<int> AddQuestion(Question question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            var sqlExpression = $"INSERT INTO Questions (question_description, question_type, point, test_id) VALUES ('{question.Description}',  {(int)question.Type}, {question.Point}, {question.TestId});" +
                "SELECT SCOPE_IDENTITY();"; 
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

        public async Task<Question> GetQuestionById(int questionId)
        {
            string sqlExpression = $"SELECT * FROM Questions Where question_id = {questionId}";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            Question question = new();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    question.TestId = (int)reader["test_id"];
                    question.Description = (string)reader["question_description"];
                    question.Type = (QuestionType)reader["question_type"];
                    question.Point = (int)reader["point"];
                    question.QuestionId = (int)reader["question_id"];
                }
            }

            return question;
        }

        public async Task<List<Question>> GetTestQuestions(int testId)
        {
            string sqlExpression = $"SELECT * FROM Questions Where test_id = {testId}";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            List<Question> questions = new List<Question>();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    Question question = new Question();
                    question.TestId = (int)reader["test_id"];
                    question.Description = (string)reader["question_description"];
                    question.Type = (QuestionType)reader["question_type"];
                    question.Point = (int)reader["point"];
                    question.QuestionId = (int)reader["question_id"];
                    questions.Add(question);
                }
            }

            return questions;
        }

        public async Task<List<QuestionStatDTO>> GetTestQuestionsDTO(int testId)
        {
            string sqlExpression = $"SELECT * FROM Questions Where test_id = {testId}";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            List<QuestionStatDTO> questions = new ();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    QuestionStatDTO question = new();
                    question.Description = (string)reader["question_description"];
                    question.Type = (QuestionType)reader["question_type"];
                    question.Point = (int)reader["point"];
                    question.QuestionId = (int)reader["question_id"];
                    questions.Add(question);
                }
            }

            return questions;
        }

        public async Task DeleteQuestion(int questionId)
        {
            string sqlExpression = $"DELETE FROM Questions WHERE question_id={questionId}";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = await command.ExecuteNonQueryAsync();
            }
        }
        public async Task UpdateQuestion(Question question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            string sqlExpression = $"UPDATE Questions SET question_description='{question.Description}', point={question.Point}, question_type={(int)question.Type},  test_id={question.TestId} WHERE question_id={question.QuestionId}";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> IsInTestQuestions(int testId)
        {
            string sqlExpression = $"select count(*) as questions_amount from [Questions] where test_id = {testId}";

            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            int questionAmount = 0;

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    questionAmount = (int)reader["questions_amount"];
                }
            }

            if (questionAmount < 1)
            {
                return false;
            }

            return true;
        }
    }
}
