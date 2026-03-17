using FeatureFlagAPI.Models;

namespace FeatureFlagAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserModel>> GetAllUsersAsync();
        Task<UserModel?> GetUserByIdAsync(int id);
        Task<UserModel?> GetUserByEmailAsync(string email);
        Task<int> CreateUserAsync(UserModel user);
        Task<bool> UpdateUserAsync(UserModel user);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<UserModel>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<UserModel>> GetUsersByRegionAsync(string region);
        Task<IEnumerable<string>> GetDistinctUserRolesAsync();
        Task<IEnumerable<string>> GetDistinctRegionsAsync();
    }
}
