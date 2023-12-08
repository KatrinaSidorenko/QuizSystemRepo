using Core.DTO;
using Core.Enums;
using Core.Models;
using Core.Settings;
using DAL.Interfaces;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Data.SqlClient;

namespace DAL.Repository
{
    public class AnswerRepository : BaseRepository, IAnswerRepository
    {
        public AnswerRepository(IOptions<ConnectionSettings> options) : base(options)
        {
        }
        public async Task AddAnswer(Answer answer)
        {
            if (answer == null)
            {
                throw new ArgumentNullException(nameof(answer));
            }

            var sqlExpression = $"INSERT INTO Answers (answer_description, is_right, question_id) VALUES ('{answer.Value}', {Convert.ToInt32(answer.IsRight)}, {answer.QuestionId});";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                var insertedId = await command.ExecuteScalarAsync();
            }
        }

        public async Task<Answer> GetAnswerById(int answerId)
        {
            string sqlExpression = $"SELECT * FROM Answers Where answer_id = {answerId}";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            Answer answer = new();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    answer.Value = (string)reader["answer_description"];
                    answer.IsRight = (int)reader["is_right"] == 1 ? true : false;
                    answer.AnswerId = (int)reader["answer_id"];
                    answer.QuestionId = (int)reader["question_id"];
                }
            }

            return answer;
        }

        public async Task<List<Answer>> GetQuestionAnswers(int questionId)
        {
            string sqlExpression = $"SELECT * FROM Answers Where question_id={questionId}";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            List<Answer> answers = new();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    Answer answer = new();
                    answer.Value = (string)reader["answer_description"];
                    answer.IsRight = (int)reader["is_right"] == 1 ? true : false;
                    answer.AnswerId = (int)reader["answer_id"];
                    answer.QuestionId = (int)reader["question_id"];
                    answers.Add(answer);
                }
            }

            return answers;
        }

        public async Task DeleteAnswer(int answerId)
        {
            string sqlExpression = $"DELETE FROM Answers WHERE answer_id={answerId}";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = await command.ExecuteNonQueryAsync();
            }
        }
        public async Task UpdateAnswer(Answer answer)
        {
            if (answer == null)
            {
                throw new ArgumentNullException(nameof(answer));
            }

            string sqlExpression = $"UPDATE Answers SET answer_description='{answer.Value}', is_right={Convert.ToInt32(answer.IsRight)}, question_id={answer.QuestionId} WHERE answer_id={answer.AnswerId}";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<AnswerStatDTO>> GetAnswersDTO(int questionId, int sharedTestId)
        {
            string sqlExpression = $"select  A.answer_description, A.answer_id, A.is_right, A.question_id,  count(distinct T.attempt_id) as choose_this_answer\r\nfrom [TestResults] T \r\nright join [Answers] A\r\non T.answer_id = A.answer_id\r\nwhere T.question_id = {questionId} " +
                $"and attempt_id in (select attempt_id from [Attempts] where shared_test_id = {sharedTestId})" +
                $"group by A.answer_description, A.answer_id, A.is_right, A.question_id\r\nunion \r\nselect A.answer_description, A.answer_id, A.is_right, A.question_id, 0 as choose_this_answer\r\nfrom [Answers] A\r\nwhere A.question_id = {questionId} and A.answer_id not in (select T.answer_id from [TestResults] T join [Attempts] Att on T.attempt_id = Att.attempt_id  where shared_test_id = {sharedTestId});";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            List<AnswerStatDTO> answers = new();

            using (connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    AnswerStatDTO answer = new();
                    answer.Value = (string)reader["answer_description"];
                    answer.IsRight = (int)reader["is_right"] == 1 ? true : false;
                    answer.AnswerId = (int)reader["answer_id"];
                    answer.QuestionId = (int)reader["question_id"];
                    answer.UserChooseAmount = (int)reader["choose_this_answer"];
                    answers.Add(answer);
                }
            }

            return answers;
        }
    }
}
