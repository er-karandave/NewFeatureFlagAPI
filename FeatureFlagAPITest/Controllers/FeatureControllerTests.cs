using FeatureFlagAPI.Controllers;
using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureFlagAPITest.Controllers
{
    public class FeatureControllerTests
    {
        private readonly Mock<IFeatureService> _mockService;
        private readonly FeatureController _controller;

        public FeatureControllerTests()
        {
            _mockService = new Mock<IFeatureService>();
            _controller = new FeatureController(_mockService.Object);
        }

        #region GetUserPermissions Tests

        [Fact]
        public async Task GetUserPermissions_ReturnsOkResult_WithPermissions()
        {
            // Arrange
            int userId = 1;
            string role = "Admin";
            string region = "North America";

            var expectedPermissions = new UserPermissionsResponseDto
            {
                UserId = userId,
                UserName = "John Doe",
                UserRole = role,
                UserRegion = region,
                EffectivePermissions = new List<EffectivePermissionDto>
                {
                    new EffectivePermissionDto
                    {
                        FeatureId = 1,
                        FeatureName = "Delete User",
                        IsGranted = true
                    }
                },
                PermissionBreakdown = new List<PermissionBreakdownDto>()
            };

            _mockService.Setup(s => s.GetUserPermissionsAsync(userId, role, region))
                .ReturnsAsync(expectedPermissions);

            // Act
            var result = await _controller.GetUserPermissions(userId, role, region);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedPermissions);
        }

        #endregion

        #region GetRolePermissions Tests

        [Fact]
        public async Task GetRolePermissions_ReturnsOkResult_WithPermissions()
        {
            // Arrange
            string roleName = "Admin";

            var expectedPermissions = new RolePermissionsResponseDto
            {
                RoleName = roleName,
                PermissionBreakdown = new List<RolePermissionBreakdownDto>
                {
                    new RolePermissionBreakdownDto
                    {
                        FeatureId = 1,
                        FeatureName = "Delete User",
                        RoleLevel = true,
                        GlobalLevel = false,
                        EffectivePermission = true
                    }
                }
            };

            _mockService.Setup(s => s.GetRolePermissionsAsync(roleName))
                .ReturnsAsync(expectedPermissions);

            // Act
            var result = await _controller.GetRolePermissions(roleName);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedPermissions);
        }

        #endregion

        #region GetRoleFeatures Tests

        [Fact]
        public async Task GetRoleFeatures_ReturnsOkResult_WithFeatures()
        {
            // Arrange
            string roleName = "Admin";

            var expectedFeatures = new List<RoleFeatureDto>
            {
                new RoleFeatureDto
                {
                    FeatureId = 1,
                    RoleName = roleName,
                    IsGranted = true
                },
                new RoleFeatureDto
                {
                    FeatureId = 2,
                    RoleName = roleName,
                    IsGranted = false
                }
            };

            _mockService.Setup(s => s.GetRoleFeaturesAsync(roleName))
                .ReturnsAsync(expectedFeatures);

            // Act
            var result = await _controller.GetRoleFeatures(roleName);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedFeatures);
        }

        #endregion

        #region AssignFeatureToRole Tests

        [Fact]
        public async Task AssignFeatureToRole_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            string roleName = "Admin";
            int featureId = 1;

            _mockService.Setup(s => s.AssignFeatureToRoleAsync(roleName, featureId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AssignFeatureToRole(roleName, featureId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task AssignFeatureToRole_ReturnsBadRequest_WhenFailure()
        {
            // Arrange
            string roleName = "Admin";
            int featureId = 1;

            _mockService.Setup(s => s.AssignFeatureToRoleAsync(roleName, featureId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.AssignFeatureToRole(roleName, featureId);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        #endregion

        #region RemoveFeatureFromRole Tests

        [Fact]
        public async Task RemoveFeatureFromRole_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            string roleName = "Admin";
            int featureId = 1;

            _mockService.Setup(s => s.RemoveFeatureFromRoleAsync(roleName, featureId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveFeatureFromRole(roleName, featureId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task RemoveFeatureFromRole_ReturnsNotFound_WhenFailure()
        {
            // Arrange
            string roleName = "Admin";
            int featureId = 1;

            _mockService.Setup(s => s.RemoveFeatureFromRoleAsync(roleName, featureId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.RemoveFeatureFromRole(roleName, featureId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region UpsertPermission Tests

        [Fact]
        public async Task UpsertPermission_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            var dto = new UpsertPermissionDto
            {
                FeatureId = 1,
                AccessLevel = "ROLE",
                AccessId = "Admin",
                Val = true
            };

            _mockService.Setup(s => s.UpsertPermissionAsync(dto.FeatureId, dto.AccessLevel, dto.AccessId, dto.Val))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpsertPermission(dto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpsertPermission_ReturnsBadRequest_WhenFailure()
        {
            // Arrange
            var dto = new UpsertPermissionDto
            {
                FeatureId = 1,
                AccessLevel = "ROLE",
                AccessId = "Admin",
                Val = true
            };

            _mockService.Setup(s => s.UpsertPermissionAsync(dto.FeatureId, dto.AccessLevel, dto.AccessId, dto.Val))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpsertPermission(dto);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        #endregion

        #region DeletePermission Tests

        [Fact]
        public async Task DeletePermission_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            int permissionId = 1;

            _mockService.Setup(s => s.DeletePermissionAsync(permissionId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeletePermission(permissionId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeletePermission_ReturnsNotFound_WhenFailure()
        {
            // Arrange
            int permissionId = 1;

            _mockService.Setup(s => s.DeletePermissionAsync(permissionId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeletePermission(permissionId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region GetPermissionBreakdown Tests

        [Fact]
        public async Task GetPermissionBreakdown_ReturnsOkResult_WithBreakdown()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            string role = "Admin";
            string region = "North America";

            var expectedBreakdown = new PermissionBreakdownDto
            {
                FeatureId = featureId,
                FeatureName = "Delete User",
                FeatureCode = "FLT_AUTH_001",
                UserLevel = false,
                RoleLevel = true,
                RegionLevel = false,
                GlobalLevel = false,
                EffectivePermission = true
            };

            _mockService.Setup(s => s.GetPermissionBreakdownAsync(featureId, userId, role, region))
                .ReturnsAsync(expectedBreakdown);

            // Act
            var result = await _controller.GetPermissionBreakdown(userId, featureId, role, region);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedBreakdown);
        }

        #endregion

        #region GetRolePermissionBreakdown Tests

        [Fact]
        public async Task GetRolePermissionBreakdown_ReturnsOkResult_WithBreakdown()
        {
            // Arrange
            string roleName = "Admin";
            int featureId = 1;

            var expectedBreakdown = new RolePermissionBreakdownDto
            {
                FeatureId = featureId,
                FeatureName = "Delete User",
                FeatureCode = "FLT_AUTH_001",
                RoleLevel = true,
                GlobalLevel = false,
                EffectivePermission = true
            };

            _mockService.Setup(s => s.GetRolePermissionBreakdownAsync(featureId, roleName))
                .ReturnsAsync(expectedBreakdown);

            // Act
            var result = await _controller.GetRolePermissionBreakdown(roleName, featureId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(expectedBreakdown);
        }

        #endregion
    }
}
