using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FeatureFlagAPI.Repositories
{
    public class FeatureRepository: IFeatureRepository
    {
        private readonly string _connectionString;

        public FeatureRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        private FeatureModel MapReaderToFeature(SqlDataReader reader)
        {
            return new FeatureModel
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                FeatureName = reader.GetString(reader.GetOrdinal("FeatureName")),
                FeatureDesc = reader.GetString(reader.GetOrdinal("FeatureDesc")),
                FeatureCode = reader.GetString(reader.GetOrdinal("FeatureCode"))
            };
        }

        public async Task<IEnumerable<FeatureModel>> GetAllFeaturesAsync()
        {
            var features = new List<FeatureModel>();
            const string query = "SELECT Id, FeatureName, FeatureDesc, FeatureCode FROM Features ORDER BY Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                features.Add(MapReaderToFeature(reader));
            }

            return features;
        }

        public async Task<FeatureModel?> GetFeatureByIdAsync(int id)
        {
            const string query = "SELECT Id, FeatureName, FeatureDesc, FeatureCode FROM Features WHERE Id = @Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapReaderToFeature(reader);
            }

            return null;
        }

        public async Task<int> CreateFeatureAsync(FeatureModel feature)
        {
            const string query = @"
                INSERT INTO Features (FeatureName, FeatureDesc, FeatureCode)
                VALUES (@FeatureName, @FeatureDesc, @FeatureCode);
                SELECT SCOPE_IDENTITY();";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@FeatureName", feature.FeatureName);
            command.Parameters.AddWithValue("@FeatureDesc", feature.FeatureDesc);
            command.Parameters.AddWithValue("@FeatureCode", feature.FeatureCode);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateFeatureAsync(FeatureModel feature)
        {
            const string query = @"
                UPDATE Features 
                SET FeatureName = @FeatureName, 
                    FeatureDesc = @FeatureDesc, 
                    FeatureCode = @FeatureCode
                WHERE Id = @Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Id", feature.Id);
            command.Parameters.AddWithValue("@FeatureName", feature.FeatureName);
            command.Parameters.AddWithValue("@FeatureDesc", feature.FeatureDesc);
            command.Parameters.AddWithValue("@FeatureCode", feature.FeatureCode);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteFeatureAsync(int id)
        {
            const string query = "DELETE FROM Features WHERE Id = @Id";

            using var connection = CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}
