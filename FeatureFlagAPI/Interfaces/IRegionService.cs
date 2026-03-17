using FeatureFlagAPI.Models;

namespace FeatureFlagAPI.Interfaces
{
    public interface IRegionService
    {
        Task<RegionListResponseDto> GetAllRegionsAsync();
        Task<IEnumerable<string>> GetDistinctRegionsAsync();
        Task<RegionResponseDto?> GetRegionByNameAsync(string regionName);
        Task<UserListResponseDto> GetUsersByRegionAsync(string regionName);
        Task<RegionStatisticsDto> GetRegionStatisticsAsync(string regionName);
        Task<RegionListResponseDto> GetAllRegionsWithStatisticsAsync();
        Task<bool> RenameRegionAsync(string oldRegionName, string newRegionName);
    }
}
