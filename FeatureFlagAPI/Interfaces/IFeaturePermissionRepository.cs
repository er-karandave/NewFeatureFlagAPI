using FeatureFlagAPI.Models;

namespace FeatureFlagAPI.Interfaces
{
    public interface IFeaturePermissionRepository
    {
        Task<IEnumerable<FeaturePermission>> GetAllPermissionsAsync();
        Task<IEnumerable<FeaturePermission>> GetPermissionsByFeatureAsync(int featureId);
        Task<FeaturePermission?> GetPermissionByIdAsync(int id);
        Task<FeaturePermission?> GetPermissionAsync(int featureId, string accessLevel, string accessId);
        Task<int> CreatePermissionAsync(FeaturePermission permission);
        Task<bool> UpdatePermissionAsync(FeaturePermission permission);
        Task<bool> DeletePermissionAsync(int id);
        Task<bool> DeletePermissionsByFeatureAsync(int featureId);

        Task<bool> GetUserEffectivePermissionAsync(int featureId, int userId, string userRole, string userRegion);
        Task<IEnumerable<FeaturePermission>> GetUserAffectingPermissionsAsync(int userId, string userRole, string userRegion);
        Task<IEnumerable<FeaturePermission>> GetPermissionsByRoleAsync(string roleName);
        Task<FeaturePermission?> GetRolePermissionAsync(int featureId, string roleName);
        Task<bool> GetRoleEffectivePermissionAsync(int featureId, string roleName);
        Task<IEnumerable<FeaturePermission>> GetRoleAffectingPermissionsAsync(string roleName);
        Task<int> GetGrantedFeaturesCountByRegionAsync(string regionName);
        Task<bool> GetRegionEffectivePermissionAsync(int featureId, string regionName);
        Task<IEnumerable<FeaturePermission>> GetRegionAffectingPermissionsAsync(string regionName);
        Task<IEnumerable<FeaturePermission>> GetPermissionsByRegionAsync(string regionName);
    }
}
