using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FeatureFlagAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private readonly IRegionService _regionService;

        public RegionController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpGet]
        public async Task<ActionResult<RegionListResponseDto>> GetAllRegions()
        {
            var result = await _regionService.GetAllRegionsAsync();
            return Ok(result);
        }

        [HttpGet("distinct")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistinctRegions()
        {
            var regions = await _regionService.GetDistinctRegionsAsync();
            return Ok(regions);
        }

        [HttpGet("{regionName}")]
        public async Task<ActionResult<RegionResponseDto>> GetRegionByName(string regionName)
        {
            var region = await _regionService.GetRegionByNameAsync(regionName);
            if (region == null)
            {
                return NotFound(new { Message = $"Region '{regionName}' not found" });
            }
            return Ok(region);
        }

        [HttpGet("{regionName}/users")]
        public async Task<ActionResult<UserListResponseDto>> GetUsersByRegion(string regionName)
        {
            var result = await _regionService.GetUsersByRegionAsync(regionName);
            return Ok(result);
        }

        [HttpGet("{regionName}/statistics")]
        public async Task<ActionResult<RegionStatisticsDto>> GetRegionStatistics(string regionName)
        {
            var stats = await _regionService.GetRegionStatisticsAsync(regionName);
            return Ok(stats);
        }

        [HttpPatch("rename")]
        public async Task<ActionResult> RenameRegion([FromBody] RenameRegionDto dto)
        {
            try
            {
                var success = await _regionService.RenameRegionAsync(dto.OldRegionName, dto.NewRegionName);
                return success ? NoContent() : NotFound(new { Message = $"Region '{dto.OldRegionName}' not found" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
