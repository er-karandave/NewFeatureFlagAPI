namespace FeatureFlagAPI.Models
{
    public class FeatureModel
    {
        public int Id { get; set; }
        public string FeatureName { get; set; }
        public string FeatureDesc { get; set; }
        public string FeatureCode { get; set; }
    }

    public class FeaturePermission
    {
        public int Id { get; set; }
        public int FeatureId { get; set; }
        public string AccessLevel { get; set; } = string.Empty;  
        public string AccessId { get; set; } = string.Empty;     
        public bool Val { get; set; }  
    }

    public class EffectivePermissionDto
    {
        public int FeatureId { get; set; }
        public string FeatureName { get; set; } = string.Empty;
        public string FeatureCode { get; set; } = string.Empty;
        public bool IsGranted { get; set; }  
        public string GrantedAtLevel { get; set; } = string.Empty;  
        public string GrantedAtId { get; set; } = string.Empty;  
    }

 public class PermissionBreakdownDto
    {
        public int FeatureId { get; set; }
        public string FeatureName { get; set; } = string.Empty;
        public string FeatureCode { get; set; } = string.Empty;
        public bool? UserLevel { get; set; }
        public bool? RoleLevel { get; set; }
        public bool? RegionLevel { get; set; }
        public bool? GlobalLevel { get; set; }

        public bool EffectivePermission { get; set; }
    }

    public class UserPermissionsResponseDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string UserRegion { get; set; } = string.Empty;
        public List<EffectivePermissionDto> EffectivePermissions { get; set; } = new();
        public List<PermissionBreakdownDto> PermissionBreakdown { get; set; } = new();
    }

    public class UpsertPermissionDto
    {
        public int FeatureId { get; set; }
        public string AccessLevel { get; set; } = string.Empty;
        public string AccessId { get; set; } = string.Empty;
        public bool Val { get; set; }
    }
}
