using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;

namespace FeatureFlagAPI.Services
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IFeaturePermissionRepository _permissionRepository;
        private readonly IFeatureRepository _featureRepository;

        public RegionService(
            IRegionRepository regionRepository,
            IFeaturePermissionRepository permissionRepository,
            IFeatureRepository featureRepository)
        {
            _regionRepository = regionRepository;
            _permissionRepository = permissionRepository;
            _featureRepository = featureRepository;
        }

        public async Task<RegionListResponseDto> GetAllRegionsAsync()
        {
            try
            {
                var regions = await _regionRepository.GetAllRegionsWithStatisticsAsync();

                return new RegionListResponseDto
                {
                    Data = regions,
                    TotalCount = regions.Count(),
                    Success = true,
                    Message = "Regions retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new RegionListResponseDto
                {
                    Data = Enumerable.Empty<RegionDto>(),
                    TotalCount = 0,
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<IEnumerable<string>> GetDistinctRegionsAsync()
        {
            return await _regionRepository.GetDistinctRegionsAsync();
        }

        public async Task<RegionResponseDto?> GetRegionByNameAsync(string regionName)
        {
            var region = await _regionRepository.GetRegionByNameAsync(regionName);
            if (region == null) return null;

            var users = await _regionRepository.GetUsersByRegionAsync(regionName);

            return new RegionResponseDto
            {
                RegionName = region.RegionName,
                AssignedUsersCount = region.AssignedUsersCount,
                FirstAssignedDate = region.FirstAssignedDate,
                IsActive = region.IsActive,
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

        public async Task<UserListResponseDto> GetUsersByRegionAsync(string regionName)
        {
            var users = await _regionRepository.GetUsersByRegionAsync(regionName);
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
                Message = $"Users in region '{regionName}' retrieved successfully"
            };
        }

        public async Task<RegionStatisticsDto> GetRegionStatisticsAsync(string regionName)
        {
            var assignedUsersCount = await _regionRepository.GetAssignedUsersCountAsync(regionName);

            var grantedFeaturesCount = await _permissionRepository.GetGrantedFeaturesCountByRegionAsync(regionName);

            var totalFeaturesCount = (await _featureRepository.GetAllFeaturesAsync()).Count();

            return new RegionStatisticsDto
            {
                RegionName = regionName,
                AssignedUsersCount = assignedUsersCount,
                GrantedFeaturesCount = grantedFeaturesCount,
                TotalFeaturesCount = totalFeaturesCount,
                PermissionCoveragePercent = totalFeaturesCount > 0
                    ? Math.Round((double)grantedFeaturesCount / totalFeaturesCount * 100, 2)
                    : 0
            };
        }

        public async Task<RegionListResponseDto> GetAllRegionsWithStatisticsAsync()
        {
            var regions = await _regionRepository.GetAllRegionsWithStatisticsAsync();

            return new RegionListResponseDto
            {
                Data = regions,
                TotalCount = regions.Count(),
                Success = true,
                Message = "Regions with statistics retrieved successfully"
            };
        }

        public async Task<bool> RenameRegionAsync(string oldRegionName, string newRegionName)
        {
            var exists = await _regionRepository.RegionExistsAsync(oldRegionName);
            if (!exists)
            {
                return false;
            }

            var newRegionExists = await _regionRepository.RegionExistsAsync(newRegionName);
            if (newRegionExists && oldRegionName != newRegionName)
            {
                throw new InvalidOperationException($"Region '{newRegionName}' already exists");
            }

            return await _regionRepository.RenameRegionAsync(oldRegionName, newRegionName);
        }
    }
}
