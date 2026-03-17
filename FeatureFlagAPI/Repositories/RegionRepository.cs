using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;
using Microsoft.Data.SqlClient;

namespace FeatureFlagAPI.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly string _connectionString;

        public RegionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<string>> GetDistinctRegionsAsync()
        {
            var regions = new List<string>();
            const string query = @"
                SELECT DISTINCT Region 
                FROM Users 
                WHERE Region IS NOT NULL AND Region != '' 
                ORDER BY Region";

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

        public async Task<RegionDto?> GetRegionByNameAsync(string regionName)
        {
            const string query = @"
                SELECT 
                    Region AS RegionName,
                    COUNT(*) AS AssignedUsersCount
                FROM Users 
                WHERE Region = @RegionName
                GROUP BY Region";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RegionName", regionName);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new RegionDto
                {
                    RegionName = reader.GetString(reader.GetOrdinal("RegionName")),
                    AssignedUsersCount = reader.GetInt32(reader.GetOrdinal("AssignedUsersCount")),
                    IsActive = true
                };
            }

            return null;
        }

        public async Task<IEnumerable<RegionDto>> GetAllRegionsWithStatisticsAsync()
        {
            var regions = new List<RegionDto>();

            const string query = @"
                SELECT 
                    Region AS RegionName,
                    COUNT(*) AS AssignedUsersCount
                FROM Users 
                WHERE Region IS NOT NULL AND Region != ''
                GROUP BY Region
                ORDER BY Region";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var region = new RegionDto
                {
                    RegionName = reader.GetString(reader.GetOrdinal("RegionName")),
                    AssignedUsersCount = reader.GetInt32(reader.GetOrdinal("AssignedUsersCount")),
                    IsActive = true
                };
                regions.Add(region);
            }

            return regions;
        }

        public async Task<IEnumerable<UserModel>> GetUsersByRegionAsync(string regionName)
        {
            var users = new List<UserModel>();
            const string query = @"
                SELECT Id, FirstName, LastName, Email, UserRole, DOB, Region 
                FROM Users 
                WHERE Region = @RegionName 
                ORDER BY Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RegionName", regionName);

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

        public async Task<int> GetAssignedUsersCountAsync(string regionName)
        {
            const string query = @"
                SELECT COUNT(*) 
                FROM Users 
                WHERE Region = @RegionName";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RegionName", regionName);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> RegionExistsAsync(string regionName)
        {
            const string query = @"
                SELECT COUNT(*) 
                FROM Users 
                WHERE Region = @RegionName";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RegionName", regionName);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<DateTime?> GetFirstAssignedDateAsync(string regionName)
        {
            const string query = @"
                SELECT MIN(CreatedDate) 
                FROM Users 
                WHERE Region = @RegionName";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RegionName", regionName);

            var result = await command.ExecuteScalarAsync();
            return result == DBNull.Value ? null : (DateTime?)result;
        }

        public async Task<bool> RenameRegionAsync(string oldRegionName, string newRegionName)
        {
            const string query = @"
                UPDATE Users 
                SET Region = @NewRegionName 
                WHERE Region = @OldRegionName";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OldRegionName", oldRegionName);
            command.Parameters.AddWithValue("@NewRegionName", newRegionName);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}
