using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlagAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<RoleListResponseDto>> GetAllRoles()
        {
            var result = await _roleService.GetAllRolesAsync();
            return Ok(result);
        }

        [HttpGet("distinct")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctRoles()
        {
            var roles = await _roleService.GetDistinctRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{roleName}")]
        public async Task<ActionResult<RoleResponseDto>> GetRoleByName(string roleName)
        {
            var role = await _roleService.GetRoleByNameAsync(roleName);
            if (role == null)
            {
                return NotFound(new { Message = $"Role '{roleName}' not found" });
            }
            return Ok(role);
        }

        [HttpGet("{roleName}/users")]
        public async Task<ActionResult<UserListResponseDto>> GetUsersByRole(string roleName)
        {
            var result = await _roleService.GetUsersByRoleAsync(roleName);
            return Ok(result);
        }

        [HttpGet("{roleName}/statistics")]
        public async Task<ActionResult<RoleStatisticsDto>> GetRoleStatistics(string roleName)
        {
            var stats = await _roleService.GetRoleStatisticsAsync(roleName);
            return Ok(stats);
        }
    }
}
