using Core.Models;
using Core.Settings;
using DAL.Interfaces;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;

namespace DAL.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IOptions<ConnectionSettings> options) : base(options)
        {
        }
        public async Task<List<User>> GetAllUsers()
        {
            string sqlExpression = "SELECT * FROM Users";

            SqlConnection connection = new SqlConnection(_connectionString);
            List<User> users = new List<User>();

            using (connection)
            {
                SqlCommand command = new SqlCommand(sqlExpression, connection);

                connection.Open();

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    User user = new User();
                    user.UserId = (int)reader["user_id"];
                    user.FirstName = (string)reader["first_name"];
                    user.LastName = (string)reader["last_name"];
                    user.Password = (string)reader["password"];
                    user.DateOfBirth = (DateTime)reader["date_of_birth"];
                    user.Email = (string)reader["email"];
                    users.Add(user);
                }
            }

            return users;
        }

        public async Task<int> AddUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var sqlExpression = $"INSERT INTO Users (first_name, last_name, password, date_of_birth, email) VALUES ('{user.FirstName}', '{user.LastName}', '{user.Password}', '{user.DateOfBirth.ToString("yyyy-MM-dd")}', '{user.Email}');"
                + "SELECT SCOPE_IDENTITY();";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand( sqlExpression, connection);
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

        public async Task<User> GetUserById(int id)
        {
            string sqlExpression = $"SELECT * FROM Users Where user_id = '{id}'";
            SqlConnection connection = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            User user = new User();

            using(connection)
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    user.UserId = (int)reader["user_id"];
                    user.FirstName = (string)reader["first_name"];
                    user.LastName = (string)reader["last_name"];
                    user.Password = (string)reader["password"];
                    user.DateOfBirth = (DateTime)reader["date_of_birth"];
                    user.Email = (string)reader["email"];
                }
            }

            return user;
        }

        public async Task UpdateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            string sqlExpression = $"UPDATE Users SET first_name='{user.FirstName}', last_name='{user.LastName}', password='{user.Password}', date_of_birth='{user.DateOfBirth.ToString("yyyy-MM-dd")}', email='{user.Email}' WHERE user_id={user.UserId}";
            SqlConnection connection = new SqlConnection(_connectionString);

            using (connection)
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int number = await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteUser(int userId)
        {
            string sqlExpression = $"DELETE FROM Users WHERE user_id={userId}";
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
