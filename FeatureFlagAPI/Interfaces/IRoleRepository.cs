using FeatureFlagAPI.Models;
using System.Data;

namespace FeatureFlagAPI.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<string>> GetDistinctRolesAsync();
        Task<RoleDto?> GetRoleByNameAsync(string roleName);
        Task<IEnumerable<RoleDto>> GetAllRolesWithStatisticsAsync();
        Task<IEnumerable<UserModel>> GetUsersByRoleAsync(string roleName);
        Task<int> GetAssignedUsersCountAsync(string roleName);
        Task<int> GetGrantedFeaturesCountAsync(string roleName);
        Task<bool> RoleExistsAsync(string roleName);
        Task<DateTime?> GetRoleCreatedDateAsync(string roleName);
    }
}
