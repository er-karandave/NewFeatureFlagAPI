using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;

namespace FeatureFlagAPI.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IFeaturePermissionRepository _permissionRepository;
        private readonly IFeatureRepository _featureRepository;

        public RoleService(
            IRoleRepository roleRepository,
            IFeaturePermissionRepository permissionRepository,
            IFeatureRepository featureRepository)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _featureRepository = featureRepository;
        }

        public async Task<RoleListResponseDto> GetAllRolesAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllRolesWithStatisticsAsync();

                return new RoleListResponseDto
                {
                    Data = roles,
                    TotalCount = roles.Count(),
                    Success = true,
                    Message = "Roles retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new RoleListResponseDto
                {
                    Data = Enumerable.Empty<RoleDto>(),
                    TotalCount = 0,
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<IEnumerable<string>> GetDistinctRolesAsync()
        {
            return await _roleRepository.GetDistinctRolesAsync();
        }

        public async Task<RoleResponseDto?> GetRoleByNameAsync(string roleName)
        {
            var role = await _roleRepository.GetRoleByNameAsync(roleName);
            if (role == null) return null;

            var users = await _roleRepository.GetUsersByRoleAsync(roleName);

            return new RoleResponseDto
            {
                RoleName = role.RoleName,
                Description = role.Description,
                AssignedUsersCount = role.AssignedUsersCount,
                GrantedFeaturesCount = role.GrantedFeaturesCount,
                AssignedUsers = users.Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    UserRole = u.UserRole,
                    DOB = u.DOB,
                    Region = u.Region
                })
            };
        }

        public async Task<UserListResponseDto> GetUsersByRoleAsync(string roleName)
        {
            var users = await _roleRepository.GetUsersByRoleAsync(roleName);
            var userDtos = users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                UserRole = u.UserRole,
                DOB = u.DOB,
                Region = u.Region
            });

            return new UserListResponseDto
            {
                Data = userDtos,
                TotalCount = userDtos.Count(),
                Success = true,
                Message = $"Users with role '{roleName}' retrieved successfully"
            };
        }

        public async Task<RoleStatisticsDto> GetRoleStatisticsAsync(string roleName)
        {
            var assignedUsersCount = await _roleRepository.GetAssignedUsersCountAsync(roleName);
            var grantedFeaturesCount = await _roleRepository.GetGrantedFeaturesCountAsync(roleName);
            var totalFeaturesCount = (await _featureRepository.GetAllFeaturesAsync()).Count();

            return new RoleStatisticsDto
            {
                RoleName = roleName,
                AssignedUsersCount = assignedUsersCount,
                GrantedFeaturesCount = grantedFeaturesCount,
                TotalFeaturesCount = totalFeaturesCount,
                PermissionCoveragePercent = totalFeaturesCount > 0
                    ? Math.Round((double)grantedFeaturesCount / totalFeaturesCount * 100, 2)
                    : 0
            };
        }

        public async Task<RoleListResponseDto> GetAllRolesWithStatisticsAsync()
        {
            var roles = await _roleRepository.GetAllRolesWithStatisticsAsync();

            return new RoleListResponseDto
            {
                Data = roles,
                TotalCount = roles.Count(),
                Success = true,
                Message = "Roles with statistics retrieved successfully"
            };
        }
    }
}
