using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;
using Microsoft.Data.SqlClient;

namespace FeatureFlagAPI.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly string _connectionString;

        public RoleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<string>> GetDistinctRolesAsync()
        {
            var roles = new List<string>();
            const string query = @"
                SELECT DISTINCT UserRole 
                FROM Users 
                WHERE UserRole IS NOT NULL AND UserRole != '' 
                ORDER BY UserRole";

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

        public async Task<RoleDto?> GetRoleByNameAsync(string roleName)
        {
            const string query = @"
                SELECT 
                    UserRole AS RoleName,
                    COUNT(*) AS AssignedUsersCount
                FROM Users 
                WHERE UserRole = @RoleName
                GROUP BY UserRole";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RoleName", roleName);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new RoleDto
                {
                    RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                    AssignedUsersCount = reader.GetInt32(reader.GetOrdinal("AssignedUsersCount")),
                    IsActive = true
                };
            }

            return null;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesWithStatisticsAsync()
        {
            var roles = new List<RoleDto>();

            const string query = @"
                SELECT 
                    UserRole AS RoleName,
                    COUNT(*) AS AssignedUsersCount
                FROM Users 
                WHERE UserRole IS NOT NULL AND UserRole != ''
                GROUP BY UserRole
                ORDER BY UserRole";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var role = new RoleDto
                {
                    RoleName = reader.GetString(reader.GetOrdinal("RoleName")),
                    AssignedUsersCount = reader.GetInt32(reader.GetOrdinal("AssignedUsersCount")),
                    IsActive = true,
                    GrantedFeaturesCount = 0  // Will be populated separately
                };
                roles.Add(role);
            }

            // Then get granted features count for each role
            foreach (var role in roles)
            {
                role.GrantedFeaturesCount = await GetGrantedFeaturesCountAsync(role.RoleName);
            }

            return roles;
        }

        public async Task<IEnumerable<UserModel>> GetUsersByRoleAsync(string roleName)
        {
            var users = new List<UserModel>();
            const string query = @"
                SELECT Id, FirstName, LastName, Email, UserRole, DOB, Region 
                FROM Users 
                WHERE UserRole = @RoleName 
                ORDER BY Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RoleName", roleName);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                users.Add(new UserModel
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    UserRole = reader.GetString(reader.GetOrdinal("UserRole")),
                    DOB = reader.IsDBNull(reader.GetOrdinal("DOB"))
                        ? null
                        : reader.GetDateTime(reader.GetOrdinal("DOB")),
                    Region = reader.GetString(reader.GetOrdinal("Region"))
                });
            }

            return users;
        }

        public async Task<int> GetAssignedUsersCountAsync(string roleName)
        {
            const string query = @"
                SELECT COUNT(*) 
                FROM Users 
                WHERE UserRole = @RoleName";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RoleName", roleName);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<int> GetGrantedFeaturesCountAsync(string roleName)
        {
            const string query = @"
                SELECT COUNT(*) 
                FROM FeaturePermissions 
                WHERE AccessLevel = 'ROLE' 
                  AND AccessId = @RoleName 
                  AND Val = 1";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RoleName", roleName);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            const string query = @"
                SELECT COUNT(*) 
                FROM Users 
                WHERE UserRole = @RoleName";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RoleName", roleName);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<DateTime?> GetRoleCreatedDateAsync(string roleName)
        {
            const string query = @"
                SELECT MIN(CreatedDate) 
                FROM Users 
                WHERE UserRole = @RoleName";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RoleName", roleName);

            var result = await command.ExecuteScalarAsync();
            return result == DBNull.Value ? null : (DateTime?)result;
        }
    }
}
