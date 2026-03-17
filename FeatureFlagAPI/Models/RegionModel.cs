namespace FeatureFlagAPI.Models
{
    public class RegionModel
    {
    }
    public class RegionDto
    {
        public string RegionName { get; set; } = string.Empty;
        public int AssignedUsersCount { get; set; }
        public DateTime? FirstAssignedDate { get; set; }  
        public bool IsActive { get; set; }  
    }

    public class RegionListResponseDto
    {
        public IEnumerable<RegionDto> Data { get; set; } = new List<RegionDto>();
        public int TotalCount { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class RegionResponseDto
    {
        public string RegionName { get; set; } = string.Empty;
        public int AssignedUsersCount { get; set; }
        public DateTime? FirstAssignedDate { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<UserResponseDto> AssignedUsers { get; set; } = new List<UserResponseDto>();
    }

    public class RegionStatisticsDto
    {
        public string RegionName { get; set; } = string.Empty;
        public int AssignedUsersCount { get; set; }
        public int GrantedFeaturesCount { get; set; }
        public int TotalFeaturesCount { get; set; }
        public double PermissionCoveragePercent { get; set; }
    }

    public class RenameRegionDto
    {
        public string OldRegionName { get; set; } = string.Empty;
        public string NewRegionName { get; set; } = string.Empty;
    }
}
