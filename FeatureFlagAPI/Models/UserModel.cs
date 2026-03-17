namespace FeatureFlagAPI.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public string Region { get; set; } = string.Empty;
    }

    public class CreateUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public string Region { get; set; } = string.Empty;
    }

    public class UpdateUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public string Region { get; set; } = string.Empty;
    }

    public class UpdateUserRoleDto
    {
        public string UserRole { get; set; } = string.Empty;
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public string Region { get; set; } = string.Empty;
    }

    public class UserListResponseDto
    {
        public IEnumerable<UserResponseDto> Data { get; set; } = new List<UserResponseDto>();
        public int TotalCount { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

}
