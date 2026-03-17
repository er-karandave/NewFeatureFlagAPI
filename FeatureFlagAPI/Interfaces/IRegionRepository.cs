using FeatureFlagAPI.Models;

namespace FeatureFlagAPI.Interfaces
{
    public interface IRegionRepository
    {
        Task<IEnumerable<string>> GetDistinctRegionsAsync();
        Task<RegionDto?> GetRegionByNameAsync(string regionName);
        Task<IEnumerable<RegionDto>> GetAllRegionsWithStatisticsAsync();
        Task<IEnumerable<UserModel>> GetUsersByRegionAsync(string regionName);
        Task<int> GetAssignedUsersCountAsync(string regionName);
        Task<bool> RegionExistsAsync(string regionName);
        Task<DateTime?> GetFirstAssignedDateAsync(string regionName);
        Task<bool> RenameRegionAsync(string oldRegionName, string newRegionName);
    }
}
