using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FeatureFlagAPI.Repositories
{
    public class FeaturePermissionRepository: IFeaturePermissionRepository
    {
        private readonly string _connectionString;

        public FeaturePermissionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        private FeaturePermission MapReaderToPermission(SqlDataReader reader)
        {
            return new FeaturePermission
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                FeatureId = reader.GetInt32(reader.GetOrdinal("FeatureId")),
                AccessLevel = reader.GetString(reader.GetOrdinal("AccessLevel")),
                AccessId = reader.GetString(reader.GetOrdinal("AccessId")),
                Val = reader.GetBoolean(reader.GetOrdinal("Val"))
            };
        }

        public async Task<IEnumerable<FeaturePermission>> GetAllPermissionsAsync()
        {
            var permissions = new List<FeaturePermission>();
            const string query = @"
                SELECT Id, FeatureId, AccessLevel, AccessId, Val 
                FROM FeaturePermissions 
                ORDER BY FeatureId, AccessLevel, AccessId";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                permissions.Add(MapReaderToPermission(reader));
            }

            return permissions;
        }

        public async Task<IEnumerable<FeaturePermission>> GetPermissionsByRoleAsync(string roleName)
        {
            var permissions = new List<FeaturePermission>();

            const string query = @"
            SELECT Id, FeatureId, AccessLevel, AccessId, Val 
            FROM FeaturePermissions 
            WHERE (AccessLevel = 'ROLE' AND AccessId = @RoleName)
               OR (AccessLevel = 'GLOBAL' AND AccessId = '1')
            ORDER BY FeatureId, 
                     CASE AccessLevel 
                         WHEN 'ROLE' THEN 1 
                         WHEN 'GLOBAL' THEN 2 
                     END";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RoleName", roleName);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                permissions.Add(MapReaderToPermission(reader));
            }

            return permissions;
        }

        public async Task<FeaturePermission?> GetRolePermissionAsync(int featureId, string roleName)
        {
            const string query = @"
            SELECT Id, FeatureId, AccessLevel, AccessId, Val 
            FROM FeaturePermissions 
            WHERE FeatureId = @FeatureId 
              AND AccessLevel = 'ROLE' 
              AND AccessId = @RoleName";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@FeatureId", featureId);
            command.Parameters.AddWithValue("@RoleName", roleName);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToPermission(reader);
            }

            return null;
        }

        public async Task<bool> GetRoleEffectivePermissionAsync(int featureId, string roleName)
        {
            var priorityOrder = new[]
            {
            new { Level = "ROLE", Id = roleName },
            new { Level = "GLOBAL", Id = "1" }
        };

            using var connection = CreateConnection();
            await connection.OpenAsync();

            foreach (var item in priorityOrder)
            {
                const string query = @"
                SELECT Val FROM FeaturePermissions 
                WHERE FeatureId = @FeatureId 
                  AND AccessLevel = @AccessLevel 
                  AND AccessId = @AccessId";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FeatureId", featureId);
                command.Parameters.AddWithValue("@AccessLevel", item.Level);
                command.Parameters.AddWithValue("@AccessId", item.Id);

                using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
                if (await reader.ReadAsync())
                {
                    return reader.GetBoolean(0);
                }
                reader.Close();
            }

            return false;
        }

        public async Task<IEnumerable<FeaturePermission>> GetRoleAffectingPermissionsAsync(string roleName)
        {
            var permissions = new List<FeaturePermission>();

            const string query = @"
            SELECT Id, FeatureId, AccessLevel, AccessId, Val 
            FROM FeaturePermissions 
            WHERE (AccessLevel = 'ROLE' AND AccessId = @RoleName)
               OR (AccessLevel = 'GLOBAL' AND AccessId = '1')
            ORDER BY FeatureId, 
                     CASE AccessLevel 
                         WHEN 'ROLE' THEN 1 
                         WHEN 'GLOBAL' THEN 2 
                     END";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RoleName", roleName);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                permissions.Add(MapReaderToPermission(reader));
            }

            return permissions;
        }

        public async Task<IEnumerable<FeaturePermission>> GetPermissionsByFeatureAsync(int featureId)
        {
            var permissions = new List<FeaturePermission>();
            const string query = @"
                SELECT Id, FeatureId, AccessLevel, AccessId, Val 
                FROM FeaturePermissions 
                WHERE FeatureId = @FeatureId 
                ORDER BY 
                    CASE AccessLevel 
                        WHEN 'USER' THEN 1 
                        WHEN 'ROLE' THEN 2 
                        WHEN 'COUNTRY' THEN 3 
                        WHEN 'GLOBAL' THEN 4 
                    END, AccessId";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@FeatureId", featureId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                permissions.Add(MapReaderToPermission(reader));
            }

            return permissions;
        }

        public async Task<FeaturePermission?> GetPermissionByIdAsync(int id)
        {
            const string query = @"
                SELECT Id, FeatureId, AccessLevel, AccessId, Val 
                FROM FeaturePermissions 
                WHERE Id = @Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToPermission(reader);
            }

            return null;
        }

        public async Task<FeaturePermission?> GetPermissionAsync(int featureId, string accessLevel, string accessId)
        {
            const string query = @"
                SELECT Id, FeatureId, AccessLevel, AccessId, Val 
                FROM FeaturePermissions 
                WHERE FeatureId = @FeatureId 
                  AND AccessLevel = @AccessLevel 
                  AND AccessId = @AccessId";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@FeatureId", featureId);
            command.Parameters.AddWithValue("@AccessLevel", accessLevel);
            command.Parameters.AddWithValue("@AccessId", accessId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToPermission(reader);
            }

            return null;
        }

        public async Task<int> CreatePermissionAsync(FeaturePermission permission)
        {
            const string query = @"
                INSERT INTO FeaturePermissions (FeatureId, AccessLevel, AccessId, Val)
                VALUES (@FeatureId, @AccessLevel, @AccessId, @Val);
                SELECT SCOPE_IDENTITY();";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@FeatureId", permission.FeatureId);
            command.Parameters.AddWithValue("@AccessLevel", permission.AccessLevel);
            command.Parameters.AddWithValue("@AccessId", permission.AccessId);
            command.Parameters.AddWithValue("@Val", permission.Val);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdatePermissionAsync(FeaturePermission permission)
        {
            const string query = @"
                UPDATE FeaturePermissions 
                SET FeatureId = @FeatureId,
                    AccessLevel = @AccessLevel,
                    AccessId = @AccessId,
                    Val = @Val
                WHERE Id = @Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", permission.Id);
            command.Parameters.AddWithValue("@FeatureId", permission.FeatureId);
            command.Parameters.AddWithValue("@AccessLevel", permission.AccessLevel);
            command.Parameters.AddWithValue("@AccessId", permission.AccessId);
            command.Parameters.AddWithValue("@Val", permission.Val);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeletePermissionAsync(int id)
        {
            const string query = "DELETE FROM FeaturePermissions WHERE Id = @Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeletePermissionsByFeatureAsync(int featureId)
        {
            const string query = "DELETE FROM FeaturePermissions WHERE FeatureId = @FeatureId";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@FeatureId", featureId);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<FeaturePermission>> GetUserAffectingPermissionsAsync(
            int userId, string userRole, string userRegion)
        {
            var permissions = new List<FeaturePermission>();

            const string query = @"
                SELECT Id, FeatureId, AccessLevel, AccessId, Val 
                FROM FeaturePermissions 
                WHERE (AccessLevel = 'USER' AND AccessId = @UserId)
                   OR (AccessLevel = 'ROLE' AND AccessId = @UserRole)
                   OR (AccessLevel = 'COUNTRY' AND AccessId = @UserRegion)
                   OR (AccessLevel = 'GLOBAL' AND AccessId = '1')
                ORDER BY FeatureId, 
                         CASE AccessLevel 
                             WHEN 'USER' THEN 1 
                             WHEN 'ROLE' THEN 2 
                             WHEN 'COUNTRY' THEN 3 
                             WHEN 'GLOBAL' THEN 4 
                         END";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId.ToString());
            command.Parameters.AddWithValue("@UserRole", userRole);
            command.Parameters.AddWithValue("@UserRegion", userRegion);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                permissions.Add(MapReaderToPermission(reader));
            }

            return permissions;
        }

        public async Task<bool> GetUserEffectivePermissionAsync(int featureId, int userId, string userRole, string userRegion)
        {
            var priorityOrder = new[]
            {
                new { Level = "USER", Id = userId.ToString() },
                new { Level = "ROLE", Id = userRole },
                new { Level = "COUNTRY", Id = userRegion },
                new { Level = "GLOBAL", Id = "1" }
            };

            using var connection = CreateConnection();
            await connection.OpenAsync();

            foreach (var item in priorityOrder)
            {
                const string query = @"
                    SELECT Val FROM FeaturePermissions 
                    WHERE FeatureId = @FeatureId 
                      AND AccessLevel = @AccessLevel 
                      AND AccessId = @AccessId";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FeatureId", featureId);
                command.Parameters.AddWithValue("@AccessLevel", item.Level);
                command.Parameters.AddWithValue("@AccessId", item.Id);

                using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
                if (await reader.ReadAsync())
                {
                    return reader.GetBoolean(0);
                }
                reader.Close();
            }

            return false;
        }

        public async Task<int> GetGrantedFeaturesCountByRegionAsync(string regionName)
        {
            const string query = @"
        SELECT COUNT(*) 
        FROM FeaturePermissions 
        WHERE AccessLevel = 'COUNTRY' 
          AND AccessId = @RegionName 
          AND Val = 1";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RegionName", regionName);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<IEnumerable<FeaturePermission>> GetPermissionsByRegionAsync(string regionName)
        {
            var permissions = new List<FeaturePermission>();

            const string query = @"
        SELECT Id, FeatureId, AccessLevel, AccessId, Val 
        FROM FeaturePermissions 
        WHERE AccessLevel = 'COUNTRY' AND AccessId = @RegionName
        ORDER BY FeatureId";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RegionName", regionName);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                permissions.Add(MapReaderToPermission(reader));
            }

            return permissions;
        }

        public async Task<IEnumerable<FeaturePermission>> GetRegionAffectingPermissionsAsync(string regionName)
        {
            var permissions = new List<FeaturePermission>();

            const string query = @"
        SELECT Id, FeatureId, AccessLevel, AccessId, Val 
        FROM FeaturePermissions 
        WHERE (AccessLevel = 'COUNTRY' AND AccessId = @RegionName)
           OR (AccessLevel = 'GLOBAL' AND AccessId = '1')
        ORDER BY FeatureId, 
                 CASE AccessLevel 
                     WHEN 'COUNTRY' THEN 1 
                     WHEN 'GLOBAL' THEN 2 
                 END";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RegionName", regionName);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                permissions.Add(MapReaderToPermission(reader));
            }

            return permissions;
        }

        public async Task<bool> GetRegionEffectivePermissionAsync(int featureId, string regionName)
        {
            var priorityOrder = new[]
            {
        new { Level = "COUNTRY", Id = regionName },
        new { Level = "GLOBAL", Id = "1" }
    };

            using var connection = CreateConnection();
            await connection.OpenAsync();

            foreach (var item in priorityOrder)
            {
                const string query = @"
            SELECT Val FROM FeaturePermissions 
            WHERE FeatureId = @FeatureId 
              AND AccessLevel = @AccessLevel 
              AND AccessId = @AccessId";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FeatureId", featureId);
                command.Parameters.AddWithValue("@AccessLevel", item.Level);
                command.Parameters.AddWithValue("@AccessId", item.Id);

                using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
                if (await reader.ReadAsync())
                {
                    return reader.GetBoolean(0);
                }
                reader.Close();
            }

            return false;
        }
    }
}
