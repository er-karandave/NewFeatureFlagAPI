using FeatureFlagAPI.Models;

namespace FeatureFlagAPI.Interfaces
{
    public interface IUser
    {
        Task<UserListResponseDto> GetAllUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto?> GetUserByEmailAsync(string email);
        Task<UserResponseDto> CreateUserAsync(CreateUserDto userDto);
        Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto userDto);
        Task<bool> DeleteUserAsync(int id);
        Task<UserListResponseDto> GetUsersByRoleAsync(string role);
        Task<UserListResponseDto> GetUsersByRegionAsync(string region);
        Task<bool> UpdateUserRoleAsync(int id, UpdateUserRoleDto dto);
        Task<IEnumerable<string>> GetDistinctUserRolesAsync();
        Task<IEnumerable<string>> GetDistinctRegionsAsync();
    }
}
