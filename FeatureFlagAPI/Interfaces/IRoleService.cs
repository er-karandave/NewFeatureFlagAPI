using FeatureFlagAPI.Models;

namespace FeatureFlagAPI.Interfaces
{
    public interface IRoleService
    {
        Task<RoleListResponseDto> GetAllRolesAsync();
        Task<IEnumerable<string>> GetDistinctRolesAsync();
        Task<RoleResponseDto?> GetRoleByNameAsync(string roleName);
        Task<UserListResponseDto> GetUsersByRoleAsync(string roleName);
        Task<RoleStatisticsDto> GetRoleStatisticsAsync(string roleName);
        Task<RoleListResponseDto> GetAllRolesWithStatisticsAsync();
    }
}
