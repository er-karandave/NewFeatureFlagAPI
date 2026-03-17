namespace FeatureFlagAPI.Models
{
    public class RoleModel
    {
        public string RoleName { get; set; }
    }

    public class RoleFeaturePermissionModel
    {
        public string RoleName { get; set; }
        public int FeatureId { get; set; }
    }

    public class RolePermissionBreakdownDto
    {
        public int FeatureId { get; set; }
        public string FeatureName { get; set; } = string.Empty;
        public string FeatureCode { get; set; } = string.Empty;
        public bool RoleLevel { get; set; }
        public bool GlobalLevel { get; set; }
        public bool RegionLevel { get; set; }
        public bool EffectivePermission { get; set; }
    }


    public class RolePermissionsResponseDto
    {
        public string RoleName { get; set; } = string.Empty;
        public List<RolePermissionBreakdownDto> PermissionBreakdown { get; set; } = new();
    }

    public class RoleDto
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int AssignedUsersCount { get; set; }
        public int GrantedFeaturesCount { get; set; }
        public DateTime? CreatedDate { get; set; }  
        public bool IsActive { get; set; }  
    }

    public class RoleListResponseDto
    {
        public IEnumerable<RoleDto> Data { get; set; } = new List<RoleDto>();
        public int TotalCount { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class RoleResponseDto
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int AssignedUsersCount { get; set; }
        public int GrantedFeaturesCount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<UserResponseDto> AssignedUsers { get; set; } = new List<UserResponseDto>();
    }

    public class RoleStatisticsDto
    {
        public string RoleName { get; set; } = string.Empty;
        public int AssignedUsersCount { get; set; }
        public int GrantedFeaturesCount { get; set; }
        public int TotalFeaturesCount { get; set; }
        public double PermissionCoveragePercent { get; set; }  
    }

    public class RoleFeatureDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int FeatureId { get; set; }
        public bool IsGranted { get; set; }
    }
}
