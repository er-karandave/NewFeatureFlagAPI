using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;
using FeatureFlagAPI.Repositories;

namespace FeatureFlagAPI.Services
{
    public class FeatureService: IFeatureService
    {
        private readonly IFeaturePermissionRepository _permissionRepository;
        private readonly IFeatureRepository _featureRepository;
        private readonly IUserRepository _userRepository;

        public FeatureService(
            IFeaturePermissionRepository permissionRepository,
            IFeatureRepository featureRepository,
            IUserRepository userRepository)
        {
            _permissionRepository = permissionRepository;
            _featureRepository = featureRepository;
            _userRepository = userRepository;
        }

        public async Task<UserPermissionsResponseDto> GetUserPermissionsAsync(
            int userId, string userRole, string userRegion)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            var allFeatures = await _featureRepository.GetAllFeaturesAsync();
            var affectingPermissions = await _permissionRepository.GetUserAffectingPermissionsAsync(
                userId, userRole, userRegion);

            var effectivePermissions = new List<EffectivePermissionDto>();
            var permissionBreakdown = new List<PermissionBreakdownDto>();

            foreach (var feature in allFeatures)
            {
                var isGranted = await _permissionRepository.GetUserEffectivePermissionAsync(
                    feature.Id, userId, userRole, userRegion);

                var grantedAt = FindGrantedAtLevel(affectingPermissions, feature.Id, isGranted);

                effectivePermissions.Add(new EffectivePermissionDto
                {
                    FeatureId = feature.Id,
                    FeatureName = feature.FeatureName,
                    FeatureCode = feature.FeatureCode,
                    IsGranted = isGranted,
                    GrantedAtLevel = grantedAt.Level,
                    GrantedAtId = grantedAt.Id
                });

                var featurePerms = affectingPermissions.Where(p => p.FeatureId == feature.Id).ToList();
                permissionBreakdown.Add(new PermissionBreakdownDto
                {
                    FeatureId = feature.Id,
                    FeatureName = feature.FeatureName,
                    FeatureCode = feature.FeatureCode,
                    UserLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "USER" && p.AccessId == userId.ToString())?.Val,
                    RoleLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "ROLE" && p.AccessId == userRole)?.Val,
                    RegionLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "COUNTRY" && p.AccessId == userRegion)?.Val,
                    GlobalLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "GLOBAL" && p.AccessId == "1")?.Val,
                    EffectivePermission = isGranted
                });
            }

            return new UserPermissionsResponseDto
            {
                UserId = userId,
                UserName = $"{user?.FirstName} {user?.LastName}" ?? $"User-{userId}",
                UserRole = userRole,
                UserRegion = userRegion,
                EffectivePermissions = effectivePermissions,
                PermissionBreakdown = permissionBreakdown
            };
        }

        public async Task<IEnumerable<RoleFeatureDto>> GetRoleFeaturesAsync(string roleName)
        {
            var allFeatures = await _featureRepository.GetAllFeaturesAsync();
            var rolePermissions = await _permissionRepository.GetPermissionsByRoleAsync(roleName);

            return allFeatures.Select(f => new RoleFeatureDto
            {
                Id = 0,
                RoleName = roleName,
                FeatureId = f.Id,
                IsGranted = rolePermissions.Any(p => p.FeatureId == f.Id && p.Val == true)
            });
        }

        //public async Task<bool> RemoveFeatureFromRoleAsync(string roleName, int featureId)
        //{
        //    var permission = await _permissionRepository.GetPermissionAsync(featureId, "ROLE", roleName);

        //    if (permission != null)
        //    {
        //        return await _permissionRepository.DeletePermissionAsync(permission.Id);
        //    }

        //    return true;
        //}

        public async Task<bool> RemoveFeatureFromRoleAsync(string roleName, int featureId)
        {
            var permission = await _permissionRepository.GetPermissionAsync(featureId, "ROLE", roleName);

            if (permission != null)
            {
                var result = await _permissionRepository.DeletePermissionAsync(permission.Id);
                return result; 
            }

            return true;
        }

        //public async Task<bool> AssignFeatureToRoleAsync(string roleName, int featureId)
        //{
        //    var existing = await _permissionRepository.GetPermissionAsync(featureId, "ROLE", roleName);

        //    if (existing != null)
        //    {
        //        existing.Val = true;
        //        return await _permissionRepository.UpdatePermissionAsync(existing);
        //    }
        //    else
        //    {
        //        var newPerm = new FeaturePermission
        //        {
        //            FeatureId = featureId,
        //            AccessLevel = "ROLE",
        //            AccessId = roleName,
        //            Val = true
        //        };
        //        await _permissionRepository.CreatePermissionAsync(newPerm);
        //        return true;
        //    }
        //}

        public async Task<bool> AssignFeatureToRoleAsync(string roleName, int featureId)
        {
            var existing = await _permissionRepository.GetPermissionAsync(featureId, "ROLE", roleName);

            if (existing != null)
            {
                existing.Val = true;
                var result = await _permissionRepository.UpdatePermissionAsync(existing);
                return result;  
            }
            else
            {
                var newPerm = new FeaturePermission
                {
                    FeatureId = featureId,
                    AccessLevel = "ROLE",
                    AccessId = roleName,
                    Val = true
                };
                await _permissionRepository.CreatePermissionAsync(newPerm);
                return true;  
            }
        }

        public async Task<PermissionBreakdownDto> GetPermissionBreakdownAsync(
            int featureId, int userId, string userRole, string userRegion)
        {
            var feature = await _featureRepository.GetFeatureByIdAsync(featureId);
            var affectingPermissions = await _permissionRepository.GetUserAffectingPermissionsAsync(
                userId, userRole, userRegion);

            var featurePerms = affectingPermissions.Where(p => p.FeatureId == featureId).ToList();
            var isGranted = await _permissionRepository.GetUserEffectivePermissionAsync(
                featureId, userId, userRole, userRegion);

            return new PermissionBreakdownDto
            {
                FeatureId = featureId,
                FeatureName = feature?.FeatureName ?? $"Feature-{featureId}",
                FeatureCode = feature?.FeatureCode ?? "",
                UserLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "USER" && p.AccessId == userId.ToString())?.Val ?? false,
                RoleLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "ROLE" && p.AccessId == userRole)?.Val ?? false,
                RegionLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "COUNTRY" && p.AccessId == userRegion)?.Val ?? false,
                GlobalLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "GLOBAL" && p.AccessId == "1")?.Val ?? false,

                EffectivePermission = isGranted
            };
        }

        //public async Task<bool> UpsertPermissionAsync(int featureId, string accessLevel, string accessId, bool val)
        //{
        //    var existing = await _permissionRepository.GetPermissionAsync(featureId, accessLevel, accessId);

        //    if (existing != null)
        //    {
        //        existing.Val = val;
        //        return await _permissionRepository.UpdatePermissionAsync(existing);
        //    }
        //    else
        //    {
        //        var newPerm = new FeaturePermission
        //        {
        //            FeatureId = featureId,
        //            AccessLevel = accessLevel,
        //            AccessId = accessId,
        //            Val = val
        //        };
        //        await _permissionRepository.CreatePermissionAsync(newPerm);
        //        return true;
        //    }
        //}

        public async Task<bool> UpsertPermissionAsync(int featureId, string accessLevel, string accessId, bool val)
        {
            var existing = await _permissionRepository.GetPermissionAsync(featureId, accessLevel, accessId);

            if (existing != null)
            {
                existing.Val = val;
                var result = await _permissionRepository.UpdatePermissionAsync(existing);
                return result; 
            }
            else
            {
                var newPerm = new FeaturePermission
                {
                    FeatureId = featureId,
                    AccessLevel = accessLevel,
                    AccessId = accessId,
                    Val = val
                };
                await _permissionRepository.CreatePermissionAsync(newPerm);
                return true; 
            }
        }

        public async Task<bool> DeletePermissionAsync(int permissionId)
        {
            return await _permissionRepository.DeletePermissionAsync(permissionId);
        }

        private (string Level, string Id) FindGrantedAtLevel(
            IEnumerable<FeaturePermission> permissions, int featureId, bool targetValue)
        {
            var priorityOrder = new[] { "USER", "ROLE", "COUNTRY", "GLOBAL" };

            foreach (var level in priorityOrder)
            {
                var perm = permissions.FirstOrDefault(p =>
                    p.FeatureId == featureId &&
                    p.AccessLevel == level &&
                    p.Val == targetValue);

                if (perm != null)
                {
                    return (level, perm.AccessId);
                }
            }

            return ("DEFAULT", "N/A");
        }

        public async Task<RolePermissionsResponseDto> GetRolePermissionsAsync(string roleName)
        {
            var allFeatures = await _featureRepository.GetAllFeaturesAsync();
            var affectingPermissions = await _permissionRepository.GetRoleAffectingPermissionsAsync(roleName);

            var permissionBreakdown = new List<RolePermissionBreakdownDto>();

            foreach (var feature in allFeatures)
            {
                var isGranted = await _permissionRepository.GetRoleEffectivePermissionAsync(
                    feature.Id, roleName);

                var featurePerms = affectingPermissions.Where(p => p.FeatureId == feature.Id).ToList();

                permissionBreakdown.Add(new RolePermissionBreakdownDto
                {
                    FeatureId = feature.Id,
                    FeatureName = feature.FeatureName,
                    FeatureCode = feature.FeatureCode,
                    RoleLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "ROLE" && p.AccessId == roleName)?.Val ?? false,
                    GlobalLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "GLOBAL" && p.AccessId == "1")?.Val ?? false,
                    EffectivePermission = isGranted
                });
            }

            return new RolePermissionsResponseDto
            {
                RoleName = roleName,
                PermissionBreakdown = permissionBreakdown
            };
        }

        public async Task<RolePermissionBreakdownDto> GetRolePermissionBreakdownAsync(
    int featureId, string roleName)
        {
            var feature = await _featureRepository.GetFeatureByIdAsync(featureId);
            var affectingPermissions = await _permissionRepository.GetRoleAffectingPermissionsAsync(roleName);

            var featurePerms = affectingPermissions.Where(p => p.FeatureId == featureId).ToList();
            var isGranted = await _permissionRepository.GetRoleEffectivePermissionAsync(
                featureId, roleName);

            return new RolePermissionBreakdownDto
            {
                FeatureId = featureId,
                FeatureName = feature?.FeatureName ?? $"Feature-{featureId}",
                FeatureCode = feature?.FeatureCode ?? "",
                RoleLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "ROLE" && p.AccessId == roleName)?.Val ?? false,
                GlobalLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "GLOBAL" && p.AccessId == "1")?.Val ?? false,

                EffectivePermission = isGranted
            };
        }

        public async Task<IEnumerable<RoleFeatureDto>> GetRegionFeaturesAsync(string regionName)
        {
            var allFeatures = await _featureRepository.GetAllFeaturesAsync();
            var regionPermissions = await _permissionRepository.GetPermissionsByRegionAsync(regionName);

            return allFeatures.Select(f => new RoleFeatureDto
            {
                Id = 0,
                RoleName = regionName,
                FeatureId = f.Id,
                IsGranted = regionPermissions.Any(p => p.FeatureId == f.Id && p.Val == true)
            });
        }

        public async Task<bool> AssignFeatureToRegionAsync(string regionName, int featureId)
        {
            var existing = await _permissionRepository.GetPermissionAsync(featureId, "COUNTRY", regionName);

            if (existing != null)
            {
                existing.Val = true;
                return await _permissionRepository.UpdatePermissionAsync(existing);
            }
            else
            {
                var newPerm = new FeaturePermission
                {
                    FeatureId = featureId,
                    AccessLevel = "COUNTRY",
                    AccessId = regionName,
                    Val = true
                };
                await _permissionRepository.CreatePermissionAsync(newPerm);
                return true;
            }
        }

        public async Task<bool> RemoveFeatureFromRegionAsync(string regionName, int featureId)
        {
            var permission = await _permissionRepository.GetPermissionAsync(featureId, "COUNTRY", regionName);

            if (permission != null)
            {
                return await _permissionRepository.DeletePermissionAsync(permission.Id);
            }

            return true;
        }

        public async Task<RolePermissionsResponseDto> GetRegionPermissionsAsync(string regionName)
        {
            var allFeatures = await _featureRepository.GetAllFeaturesAsync();
            var affectingPermissions = await _permissionRepository.GetRegionAffectingPermissionsAsync(regionName);

            var permissionBreakdown = new List<RolePermissionBreakdownDto>();

            foreach (var feature in allFeatures)
            {
                var isGranted = await _permissionRepository.GetRegionEffectivePermissionAsync(feature.Id, regionName);
                var featurePerms = affectingPermissions.Where(p => p.FeatureId == feature.Id).ToList();

                permissionBreakdown.Add(new RolePermissionBreakdownDto
                {
                    FeatureId = feature.Id,
                    FeatureName = feature.FeatureName,
                    FeatureCode = feature.FeatureCode,
                    GlobalLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "GLOBAL" && p.AccessId == "1")?.Val ?? false,
                    EffectivePermission = isGranted
                });
            }

            return new RolePermissionsResponseDto
            {
                RoleName = regionName,
                PermissionBreakdown = permissionBreakdown
            };
        }

        public async Task<RolePermissionBreakdownDto> GetRegionPermissionBreakdownAsync(
    int featureId, string regionName)
        {
            var feature = await _featureRepository.GetFeatureByIdAsync(featureId);
            var affectingPermissions = await _permissionRepository.GetRegionAffectingPermissionsAsync(regionName);

            var featurePerms = affectingPermissions.Where(p => p.FeatureId == featureId).ToList();
            var isGranted = await _permissionRepository.GetRegionEffectivePermissionAsync(featureId, regionName);

            return new RolePermissionBreakdownDto
            {
                FeatureId = featureId,
                FeatureName = feature?.FeatureName ?? $"Feature-{featureId}",
                FeatureCode = feature?.FeatureCode ?? "",

                RoleLevel = false,  // ✅ Not applicable for regions
                RegionLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "COUNTRY" && p.AccessId == regionName)?.Val ?? false,  // ✅ ADD: Region level
                GlobalLevel = featurePerms.FirstOrDefault(p => p.AccessLevel == "GLOBAL" && p.AccessId == "1")?.Val ?? false,

                EffectivePermission = isGranted
            };
        }


    }
}
