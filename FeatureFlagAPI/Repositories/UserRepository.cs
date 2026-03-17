using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;
using Microsoft.Data.SqlClient;

namespace FeatureFlagAPI.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        private UserModel MapReaderToUser(SqlDataReader reader)
        {
            return new UserModel
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                UserRole = reader.GetString(reader.GetOrdinal("UserRole")),
                DOB = reader.IsDBNull(reader.GetOrdinal("DOB")) ? null : reader.GetDateTime(reader.GetOrdinal("DOB")),
                Region = reader.GetString(reader.GetOrdinal("Region"))
            };
        }

        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            var users = new List<UserModel>();
            const string query = "SELECT Id, FirstName, LastName, Email, UserRole, DOB, Region FROM Users ORDER BY Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(MapReaderToUser(reader));
            }

            return users;
        }

        public async Task<UserModel?> GetUserByIdAsync(int id)
        {
            const string query = "SELECT Id, FirstName, LastName, Email, UserRole, DOB, Region FROM Users WHERE Id = @Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToUser(reader);
            }

            return null;
        }

        public async Task<UserModel?> GetUserByEmailAsync(string email)
        {
            const string query = "SELECT Id, FirstName, LastName, Email, UserRole, DOB, Region FROM Users WHERE Email = @Email";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToUser(reader);
            }

            return null;
        }

        public async Task<int> CreateUserAsync(UserModel user)
        {
            const string query = @"
                INSERT INTO Users (FirstName, LastName, Email, UserRole, DOB, Region)
                VALUES (@FirstName, @LastName, @Email, @UserRole, @DOB, @Region);
                SELECT SCOPE_IDENTITY();";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@UserRole", user.UserRole);
            command.Parameters.AddWithValue("@DOB", (object?)user.DOB ?? DBNull.Value);
            command.Parameters.AddWithValue("@Region", user.Region);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateUserAsync(UserModel user)
        {
            const string query = @"
                UPDATE Users 
                SET FirstName = @FirstName, 
                    LastName = @LastName, 
                    Email = @Email, 
                    UserRole = @UserRole, 
                    DOB = @DOB, 
                    Region = @Region
                WHERE Id = @Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@UserRole", user.UserRole);
            command.Parameters.AddWithValue("@DOB", (object?)user.DOB ?? DBNull.Value);
            command.Parameters.AddWithValue("@Region", user.Region);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            const string query = "DELETE FROM Users WHERE Id = @Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<UserModel>> GetUsersByRoleAsync(string role)
        {
            var users = new List<UserModel>();
            const string query = "SELECT Id, FirstName, LastName, Email, UserRole, DOB, Region FROM Users WHERE UserRole = @UserRole ORDER BY Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserRole", role);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(MapReaderToUser(reader));
            }

            return users;
        }

        public async Task<IEnumerable<string>> GetDistinctUserRolesAsync()
        {
            var roles = new List<string>();
            const string query = "SELECT DISTINCT UserRole FROM Users WHERE UserRole IS NOT NULL AND UserRole != '' ORDER BY UserRole";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                roles.Add(reader.GetString(0));
            }
            return roles;
        }

        public async Task<IEnumerable<string>> GetDistinctRegionsAsync()
        {
            var regions = new List<string>();
            const string query = "SELECT DISTINCT Region FROM Users WHERE Region IS NOT NULL AND Region != '' ORDER BY Region";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                regions.Add(reader.GetString(0));
            }

            return regions;
        }

        public async Task<IEnumerable<UserModel>> GetUsersByRegionAsync(string region)
        {
            var users = new List<UserModel>();
            const string query = "SELECT Id, FirstName, LastName, Email, UserRole, DOB, Region FROM Users WHERE Region = @Region ORDER BY Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Region", region);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(MapReaderToUser(reader));
            }

            return users;
        }
    }
}
