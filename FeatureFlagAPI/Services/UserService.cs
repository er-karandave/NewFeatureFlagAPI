using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;
using Microsoft.Data.SqlClient;

namespace FeatureFlagAPI.Services
{
    public class UserService :IUser
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserListResponseDto> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                var userDtos = users.Select(MapToResponseDto);

                return new UserListResponseDto
                {
                    Data = userDtos,
                    TotalCount = userDtos.Count(),
                    Success = true,
                    Message = "Users retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new UserListResponseDto
                {
                    Data = Enumerable.Empty<UserResponseDto>(),
                    TotalCount = 0,
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return user != null ? MapToResponseDto(user) : null;
        }

        public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            return user != null ? MapToResponseDto(user) : null;
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto userDto)
        {
            var user = new UserModel
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                UserRole = userDto.UserRole,
                DOB = userDto.DOB,
                Region = userDto.Region
            };

            var newId = await _userRepository.CreateUserAsync(user);
            user.Id = newId;

            return MapToResponseDto(user);
        }

        public async Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto userDto)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            existingUser.FirstName = userDto.FirstName;
            existingUser.LastName = userDto.LastName;
            existingUser.Email = userDto.Email;
            existingUser.UserRole = userDto.UserRole;
            existingUser.DOB = userDto.DOB;
            existingUser.Region = userDto.Region;

            var updated = await _userRepository.UpdateUserAsync(existingUser);
            return updated ? MapToResponseDto(existingUser) : null;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        public async Task<UserListResponseDto> GetUsersByRoleAsync(string role)
        {
            var users = await _userRepository.GetUsersByRoleAsync(role);
            var userDtos = users.Select(MapToResponseDto);

            return new UserListResponseDto
            {
                Data = userDtos,
                TotalCount = userDtos.Count(),
                Success = true,
                Message = $"Users with role '{role}' retrieved successfully"
            };
        }

        public async Task<UserListResponseDto> GetUsersByRegionAsync(string region)
        {
            var users = await _userRepository.GetUsersByRegionAsync(region);
            var userDtos = users.Select(MapToResponseDto);

            return new UserListResponseDto
            {
                Data = userDtos,
                TotalCount = userDtos.Count(),
                Success = true,
                Message = $"Users in region '{region}' retrieved successfully"
            };
        }

        public async Task<bool> UpdateUserRoleAsync(int id, UpdateUserRoleDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            user.UserRole = dto.UserRole;
            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<IEnumerable<string>> GetDistinctUserRolesAsync()
        {
            return await _userRepository.GetDistinctUserRolesAsync();
        }

        public async Task<IEnumerable<string>> GetDistinctRegionsAsync()
        {
            return await _userRepository.GetDistinctRegionsAsync();
        }

        private UserResponseDto MapToResponseDto(UserModel user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserRole = user.UserRole,
                DOB = user.DOB,
                Region = user.Region
            };
        }
    }
}
