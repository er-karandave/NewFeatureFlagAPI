using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;
using FeatureFlagAPI.Repositories;
using FeatureFlagAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlagAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUser _userService;

        public UserController(IUser userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<UserListResponseDto>> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"User with ID {id} not found" });
            }
            return Ok(user);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserResponseDto>> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new { Message = $"User with email {email} not found" });
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserDto userDto)
        {
            var createdUser = await _userService.CreateUserAsync(userDto);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, userDto);
            if (updatedUser == null)
            {
                return NotFound(new { Message = $"User with ID {id} not found" });
            }
            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
            {
                return NotFound(new { Message = $"User with ID {id} not found" });
            }
            return NoContent();
        }

        [HttpGet("role/{role}")]
        public async Task<ActionResult<UserListResponseDto>> GetUsersByRole(string role)
        {
            var result = await _userService.GetUsersByRoleAsync(role);
            return Ok(result);
        }

        [HttpGet("region/{region}")]
        public async Task<ActionResult<UserListResponseDto>> GetUsersByRegion(string region)
        {
            var result = await _userService.GetUsersByRegionAsync(region);
            return Ok(result);
        }

        [HttpPatch("{id}/role")]
        public async Task<ActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDto dto)
        {
            var updated = await _userService.UpdateUserRoleAsync(id, dto);
            if (!updated)
            {
                return NotFound(new { Message = $"User with ID {id} not found" });
            }
            return NoContent();
        }

        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctUserRoles()
        {
            var roles = await _userService.GetDistinctUserRolesAsync();
            return Ok(roles);
        }

        [HttpGet("regions")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctRegions()
        {
            var regions = await _userService.GetDistinctRegionsAsync();
            return Ok(regions);
        }

    }
}
