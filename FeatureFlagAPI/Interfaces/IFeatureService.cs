using FeatureFlagAPI.Models;

namespace FeatureFlagAPI.Interfaces
{
    public interface IFeatureService
    {
        Task<UserPermissionsResponseDto> GetUserPermissionsAsync(int userId, string userRole, string userRegion);
        Task<PermissionBreakdownDto> GetPermissionBreakdownAsync(int featureId, int userId, string userRole, string userRegion);
        Task<bool> UpsertPermissionAsync(int featureId, string accessLevel, string accessId, bool val);
        Task<bool> DeletePermissionAsync(int permissionId);
        Task<RolePermissionsResponseDto> GetRolePermissionsAsync(string roleName);
        Task<RolePermissionBreakdownDto> GetRolePermissionBreakdownAsync(int featureId, string roleName);
        Task<IEnumerable<RoleFeatureDto>> GetRoleFeaturesAsync(string roleName);
        Task<bool> AssignFeatureToRoleAsync(string roleName, int featureId);
        Task<bool> RemoveFeatureFromRoleAsync(string roleName, int featureId);
        Task<RolePermissionsResponseDto> GetRegionPermissionsAsync(string regionName);
        Task<RolePermissionBreakdownDto> GetRegionPermissionBreakdownAsync(int featureId, string regionName);
        Task<IEnumerable<RoleFeatureDto>> GetRegionFeaturesAsync(string regionName);
        Task<bool> AssignFeatureToRegionAsync(string regionName, int featureId);
        Task<bool> RemoveFeatureFromRegionAsync(string regionName, int featureId);
    }
}
