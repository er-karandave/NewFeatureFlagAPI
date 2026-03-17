using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlagAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureController : ControllerBase
    {
        private readonly IFeatureService _permissionService;

        public FeatureController(IFeatureService permissionService)
        {
            _permissionService = permissionService;
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<UserPermissionsResponseDto>> GetUserPermissions(
            int userId,
            [FromQuery] string role,
            [FromQuery] string region)
        {
            var permissions = await _permissionService.GetUserPermissionsAsync(userId, role, region);
            return Ok(permissions);
        }

        [HttpGet("user/{userId}/feature/{featureId}")]
        public async Task<ActionResult<PermissionBreakdownDto>> GetPermissionBreakdown(
            int userId,
            int featureId,
            [FromQuery] string role,
            [FromQuery] string region)
        {
            var breakdown = await _permissionService.GetPermissionBreakdownAsync(
                featureId, userId, role, region);
            return Ok(breakdown);
        }

        [HttpPost]
        public async Task<ActionResult> UpsertPermission([FromBody] UpsertPermissionDto dto)
        {
            var success = await _permissionService.UpsertPermissionAsync(
                dto.FeatureId, dto.AccessLevel, dto.AccessId, dto.Val);
            return success ? NoContent() : BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePermission(int id)
        {
            var deleted = await _permissionService.DeletePermissionAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("role/{roleName}")]
        public async Task<ActionResult<RolePermissionsResponseDto>> GetRolePermissions(
        string roleName)
        {
            var permissions = await _permissionService.GetRolePermissionsAsync(roleName);
            return Ok(permissions);
        }

        [HttpGet("role/{roleName}/feature/{featureId}")]
        public async Task<ActionResult<RolePermissionBreakdownDto>> GetRolePermissionBreakdown(
        string roleName,
        int featureId)
        {
            var breakdown = await _permissionService.GetRolePermissionBreakdownAsync(
                featureId, roleName);
            return Ok(breakdown);
        }

        [HttpGet("role/{roleName}/features")]
        public async Task<ActionResult<IEnumerable<RoleFeatureDto>>> GetRoleFeatures(string roleName)
        {
            var features = await _permissionService.GetRoleFeaturesAsync(roleName);
            return Ok(features);
        }

        [HttpPost("role/{roleName}/features/{featureId}")]
        public async Task<ActionResult> AssignFeatureToRole(string roleName, int featureId)
        {
            var success = await _permissionService.AssignFeatureToRoleAsync(roleName, featureId);
            return success ? NoContent() : BadRequest();
        }

        [HttpDelete("role/{roleName}/features/{featureId}")]
        public async Task<ActionResult> RemoveFeatureFromRole(string roleName, int featureId)
        {
            var success = await _permissionService.RemoveFeatureFromRoleAsync(roleName, featureId);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("region/{regionName}")]
        public async Task<ActionResult<RolePermissionsResponseDto>> GetRegionPermissions(string regionName)
        {
            var permissions = await _permissionService.GetRegionPermissionsAsync(regionName);
            return Ok(permissions);
        }

        [HttpGet("region/{regionName}/feature/{featureId}")]
        public async Task<ActionResult<RolePermissionBreakdownDto>> GetRegionPermissionBreakdown(
    string regionName,
    int featureId)
        {
            var breakdown = await _permissionService.GetRegionPermissionBreakdownAsync(featureId, regionName);
            return Ok(breakdown);
        }

        [HttpGet("region/{regionName}/features")]
        public async Task<ActionResult<IEnumerable<RoleFeatureDto>>> GetRegionFeatures(string regionName)
        {
            var features = await _permissionService.GetRegionFeaturesAsync(regionName);
            return Ok(features);
        }

        [HttpPost("region/{regionName}/features/{featureId}")]
        public async Task<ActionResult> AssignFeatureToRegion(string regionName, int featureId)
        {
            var success = await _permissionService.AssignFeatureToRegionAsync(regionName, featureId);
            return success ? NoContent() : BadRequest();
        }

        [HttpDelete("region/{regionName}/features/{featureId}")]
        public async Task<ActionResult> RemoveFeatureFromRegion(string regionName, int featureId)
        {
            var success = await _permissionService.RemoveFeatureFromRegionAsync(regionName, featureId);
            return success ? NoContent() : NotFound();
        }
    }
}
