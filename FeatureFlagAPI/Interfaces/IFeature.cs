using FeatureFlagAPI.Models;

namespace FeatureFlagAPI.Interfaces
{
    public interface IFeature
    {
        Task<IEnumerable<FeaturePermission>> GetPermissionsByFeatureAsync(int featureId);
        Task<FeaturePermission?> GetPermissionAsync(int featureId, string accessLevel, string accessId);

        Task<bool> GetUserEffectivePermissionAsync(int featureId, int userId, string userRole, string userRegion);

        Task<IEnumerable<FeaturePermission>> GetUserAffectingPermissionsAsync(int userId, string userRole, string userRegion);

    }
}
