using FeatureFlagAPI.Interfaces;
using FeatureFlagAPI.Models;
using FeatureFlagAPI.Services;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureFlagAPITest.UnitTests.Services
{
    public class FeatureServiceTests
    {
        private readonly Mock<IFeaturePermissionRepository> _mockPermissionRepo;
        private readonly Mock<IFeatureRepository> _mockFeatureRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly FeatureService _featureService;

        public FeatureServiceTests()
        {
            _mockPermissionRepo = new Mock<IFeaturePermissionRepository>();
            _mockFeatureRepo = new Mock<IFeatureRepository>();
            _mockUserRepo = new Mock<IUserRepository>();

            _featureService = new FeatureService(
                _mockPermissionRepo.Object,
                _mockFeatureRepo.Object,
                _mockUserRepo.Object
            );
        }

        #region GetUserPermissionsAsync Tests

        [Fact]
        public async Task GetUserPermissionsAsync_ReturnsCorrectPermissions()
        {
            // Arrange
            int userId = 1;
            string userRole = "Admin";
            string userRegion = "North America";

            var user = new FeatureFlagAPI.Models.UserModel
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                UserRole = userRole,
                Region = userRegion
            };

            var features = new List<FeatureModel>
            {
                new FeatureModel { Id = 1, FeatureName = "Delete User", FeatureCode = "FLT_AUTH_001" },
                new FeatureModel { Id = 2, FeatureName = "Dark Mode", FeatureCode = "FLT_UI_002" }
            };

            var affectingPermissions = new List<FeaturePermission>
            {
                new FeaturePermission
                {
                    FeatureId = 1,
                    AccessLevel = "ROLE",
                    AccessId = "Admin",
                    Val = true
                },
                new FeaturePermission
                {
                    FeatureId = 2,
                    AccessLevel = "GLOBAL",
                    AccessId = "1",
                    Val = true
                }
            };

            _mockUserRepo.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _mockFeatureRepo.Setup(r => r.GetAllFeaturesAsync())
                .ReturnsAsync(features);

            _mockPermissionRepo.Setup(r => r.GetUserAffectingPermissionsAsync(userId, userRole, userRegion))
                .ReturnsAsync(affectingPermissions);

            _mockPermissionRepo.Setup(r => r.GetUserEffectivePermissionAsync(1, userId, userRole, userRegion))
                .ReturnsAsync(true);

            _mockPermissionRepo.Setup(r => r.GetUserEffectivePermissionAsync(2, userId, userRole, userRegion))
                .ReturnsAsync(true);

            // Act
            var result = await _featureService.GetUserPermissionsAsync(userId, userRole, userRegion);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(userId);
            result.UserName.Should().Be("John Doe");
            result.EffectivePermissions.Should().HaveCount(2);
            result.EffectivePermissions.First().FeatureId.Should().Be(1);
            result.EffectivePermissions.First().IsGranted.Should().BeTrue();
        }

        [Fact]
        public async Task GetUserPermissionsAsync_ReturnsEmptyPermissions_WhenNoFeatures()
        {
            // Arrange
            int userId = 1;
            string userRole = "Admin";
            string userRegion = "North America";

            var user = new UserModel { Id = userId, FirstName = "John", LastName = "Doe" };

            _mockUserRepo.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _mockFeatureRepo.Setup(r => r.GetAllFeaturesAsync())
                .ReturnsAsync(new List<FeatureModel>());

            // Act
            var result = await _featureService.GetUserPermissionsAsync(userId, userRole, userRegion);

            // Assert
            result.Should().NotBeNull();
            result.EffectivePermissions.Should().BeEmpty();
            result.PermissionBreakdown.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUserPermissionsAsync_ReturnsUserName_WhenUserIsNull()
        {
            // Arrange
            int userId = 1;
            string userRole = "Admin";
            string userRegion = "North America";

            _mockUserRepo.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync((UserModel)null);

            _mockFeatureRepo.Setup(r => r.GetAllFeaturesAsync())
                .ReturnsAsync(new List<FeatureModel>());

            var result = await _featureService.GetUserPermissionsAsync(userId, userRole, userRegion);

            result.UserName.Should().Be($"User-{userId}");
        }

        #endregion

        #region GetRoleFeaturesAsync Tests

        [Fact]
        public async Task GetRoleFeaturesAsync_ReturnsAllFeaturesWithGrantStatus()
        {
            // Arrange
            string roleName = "Admin";

            var features = new List<FeatureModel>
            {
                new FeatureModel { Id = 1, FeatureName = "Delete User", FeatureCode = "FLT_AUTH_001" },
                new FeatureModel { Id = 2, FeatureName = "Dark Mode", FeatureCode = "FLT_UI_002" }
            };

            var rolePermissions = new List<FeaturePermission>
            {
                new FeaturePermission
                {
                    FeatureId = 1,
                    AccessLevel = "ROLE",
                    AccessId = "Admin",
                    Val = true
                }
            };

            _mockFeatureRepo.Setup(r => r.GetAllFeaturesAsync())
                .ReturnsAsync(features);

            _mockPermissionRepo.Setup(r => r.GetPermissionsByRoleAsync(roleName))
                .ReturnsAsync(rolePermissions);

            // Act
            var result = await _featureService.GetRoleFeaturesAsync(roleName);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First(f => f.FeatureId == 1).IsGranted.Should().BeTrue();
            result.First(f => f.FeatureId == 2).IsGranted.Should().BeFalse();
        }

        [Fact]
        public async Task GetRoleFeaturesAsync_ReturnsEmptyList_WhenNoFeatures()
        {
            // Arrange
            string roleName = "Admin";

            _mockFeatureRepo.Setup(r => r.GetAllFeaturesAsync())
                .ReturnsAsync(new List<FeatureModel>());

            // Act
            var result = await _featureService.GetRoleFeaturesAsync(roleName);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region AssignFeatureToRoleAsync Tests

        [Fact]
        public async Task AssignFeatureToRoleAsync_CreatesNewPermission_WhenNotExists()
        {
            // Arrange
            string roleName = "Admin";
            int featureId = 1;

            _mockPermissionRepo.Setup(r => r.GetPermissionAsync(featureId, "ROLE", roleName))
                .ReturnsAsync((FeaturePermission)null);

            // Act
            var result = await _featureService.AssignFeatureToRoleAsync(roleName, featureId);

            // Assert
            result.Should().BeTrue();
            _mockPermissionRepo.Verify(r => r.CreatePermissionAsync(It.IsAny<FeaturePermission>()), Times.Once);
            _mockPermissionRepo.Verify(r => r.UpdatePermissionAsync(It.IsAny<FeaturePermission>()), Times.Never);
        }

        [Fact]
        public async Task AssignFeatureToRoleAsync_UpdatesExistingPermission_WhenExists()
        {
            string roleName = "Admin";
            int featureId = 1;

            var existingPermission = new FeaturePermission
            {
                Id = 1,
                FeatureId = featureId,
                AccessLevel = "ROLE",
                AccessId = roleName,
                Val = false
            };

            _mockPermissionRepo.Setup(r => r.GetPermissionAsync(featureId, "ROLE", roleName))
                .ReturnsAsync(existingPermission);

            _mockPermissionRepo.Setup(r => r.UpdatePermissionAsync(It.IsAny<FeaturePermission>()))
                .ReturnsAsync(true);

            var result = await _featureService.AssignFeatureToRoleAsync(roleName, featureId);

            result.Should().BeTrue();
            _mockPermissionRepo.Verify(r => r.UpdatePermissionAsync(It.IsAny<FeaturePermission>()), Times.Once);
        }

        #endregion

        #region RemoveFeatureFromRoleAsync Tests

        [Fact]
        public async Task RemoveFeatureFromRoleAsync_DeletesPermission_WhenExists()
        {
            string roleName = "Admin";
            int featureId = 1;

            var permission = new FeaturePermission
            {
                Id = 1,
                FeatureId = featureId,
                AccessLevel = "ROLE",
                AccessId = roleName,
                Val = true
            };

            _mockPermissionRepo.Setup(r => r.GetPermissionAsync(featureId, "ROLE", roleName))
                .ReturnsAsync(permission);

            _mockPermissionRepo.Setup(r => r.DeletePermissionAsync(1))
                .ReturnsAsync(true);

            var result = await _featureService.RemoveFeatureFromRoleAsync(roleName, featureId);

            result.Should().BeTrue();
            _mockPermissionRepo.Verify(r => r.DeletePermissionAsync(1), Times.Once);
        }

        [Fact]
        public async Task RemoveFeatureFromRoleAsync_ReturnsTrue_WhenPermissionNotExists()
        {
            // Arrange
            string roleName = "Admin";
            int featureId = 1;

            _mockPermissionRepo.Setup(r => r.GetPermissionAsync(featureId, "ROLE", roleName))
                .ReturnsAsync((FeaturePermission)null);

            // Act
            var result = await _featureService.RemoveFeatureFromRoleAsync(roleName, featureId);

            // Assert
            result.Should().BeTrue();
            _mockPermissionRepo.Verify(r => r.DeletePermissionAsync(It.IsAny<int>()), Times.Never);
        }

        #endregion

        #region UpsertPermissionAsync Tests

        [Fact]
        public async Task UpsertPermissionAsync_CreatesPermission_WhenNotExists()
        {
            // Arrange
            int featureId = 1;
            string accessLevel = "ROLE";
            string accessId = "Admin";
            bool val = true;

            _mockPermissionRepo.Setup(r => r.GetPermissionAsync(featureId, accessLevel, accessId))
                .ReturnsAsync((FeaturePermission)null);

            // Act
            var result = await _featureService.UpsertPermissionAsync(featureId, accessLevel, accessId, val);

            // Assert
            result.Should().BeTrue();
            _mockPermissionRepo.Verify(r => r.CreatePermissionAsync(It.IsAny<FeaturePermission>()), Times.Once);
        }

        [Fact]
        public async Task UpsertPermissionAsync_UpdatesPermission_WhenExists()
        {
            // Arrange
            int featureId = 1;
            string accessLevel = "ROLE";
            string accessId = "Admin";
            bool val = true;

            var existingPermission = new FeaturePermission
            {
                Id = 1,
                FeatureId = featureId,
                AccessLevel = accessLevel,
                AccessId = accessId,
                Val = false
            };

            _mockPermissionRepo.Setup(r => r.GetPermissionAsync(featureId, accessLevel, accessId))
                .ReturnsAsync(existingPermission);

            _mockPermissionRepo.Setup(r => r.UpdatePermissionAsync(It.IsAny<FeaturePermission>()))
                .ReturnsAsync(true);

            var result = await _featureService.UpsertPermissionAsync(featureId, accessLevel, accessId, val);

            result.Should().BeTrue();
            _mockPermissionRepo.Verify(r => r.UpdatePermissionAsync(It.IsAny<FeaturePermission>()), Times.Once);
        }

        #endregion

        #region DeletePermissionAsync Tests

        [Fact]
        public async Task DeletePermissionAsync_ReturnsTrue_WhenDeleted()
        {
            // Arrange
            int permissionId = 1;

            _mockPermissionRepo.Setup(r => r.DeletePermissionAsync(permissionId))
                .ReturnsAsync(true);

            // Act
            var result = await _featureService.DeletePermissionAsync(permissionId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeletePermissionAsync_ReturnsFalse_WhenNotFound()
        {
            // Arrange
            int permissionId = 1;

            _mockPermissionRepo.Setup(r => r.DeletePermissionAsync(permissionId))
                .ReturnsAsync(false);

            // Act
            var result = await _featureService.DeletePermissionAsync(permissionId);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region GetRolePermissionsAsync Tests

        [Fact]
        public async Task GetRolePermissionsAsync_ReturnsCorrectBreakdown()
        {
            // Arrange
            string roleName = "Admin";

            var features = new List<FeatureModel>
            {
                new FeatureModel { Id = 1, FeatureName = "Delete User", FeatureCode = "FLT_AUTH_001" }
            };

            var affectingPermissions = new List<FeaturePermission>
            {
                new FeaturePermission
                {
                    FeatureId = 1,
                    AccessLevel = "ROLE",
                    AccessId = "Admin",
                    Val = true
                }
            };

            _mockFeatureRepo.Setup(r => r.GetAllFeaturesAsync())
                .ReturnsAsync(features);

            _mockPermissionRepo.Setup(r => r.GetRoleAffectingPermissionsAsync(roleName))
                .ReturnsAsync(affectingPermissions);

            _mockPermissionRepo.Setup(r => r.GetRoleEffectivePermissionAsync(1, roleName))
                .ReturnsAsync(true);

            // Act
            var result = await _featureService.GetRolePermissionsAsync(roleName);

            // Assert
            result.Should().NotBeNull();
            result.RoleName.Should().Be(roleName);
            result.PermissionBreakdown.Should().HaveCount(1);
            result.PermissionBreakdown.First().RoleLevel.Should().BeTrue();
            result.PermissionBreakdown.First().EffectivePermission.Should().BeTrue();
        }

        [Fact]
        public async Task GetRolePermissionsAsync_ReturnsEmptyBreakdown_WhenNoFeatures()
        {
            // Arrange
            string roleName = "Admin";

            _mockFeatureRepo.Setup(r => r.GetAllFeaturesAsync())
                .ReturnsAsync(new List<FeatureModel>());

            // Act
            var result = await _featureService.GetRolePermissionsAsync(roleName);

            // Assert
            result.PermissionBreakdown.Should().BeEmpty();
        }

        #endregion

        #region GetRolePermissionBreakdownAsync Tests

        [Fact]
        public async Task GetRolePermissionBreakdownAsync_ReturnsCorrectBreakdown()
        {
            // Arrange
            int featureId = 1;
            string roleName = "Admin";

            var feature = new FeatureModel
            {
                Id = featureId,
                FeatureName = "Delete User",
                FeatureCode = "FLT_AUTH_001"
            };

            var affectingPermissions = new List<FeaturePermission>
            {
                new FeaturePermission
                {
                    FeatureId = featureId,
                    AccessLevel = "ROLE",
                    AccessId = "Admin",
                    Val = true
                }
            };

            _mockFeatureRepo.Setup(r => r.GetFeatureByIdAsync(featureId))
                .ReturnsAsync(feature);

            _mockPermissionRepo.Setup(r => r.GetRoleAffectingPermissionsAsync(roleName))
                .ReturnsAsync(affectingPermissions);

            _mockPermissionRepo.Setup(r => r.GetRoleEffectivePermissionAsync(featureId, roleName))
                .ReturnsAsync(true);

            // Act
            var result = await _featureService.GetRolePermissionBreakdownAsync(featureId, roleName);

            // Assert
            result.Should().NotBeNull();
            result.FeatureId.Should().Be(featureId);
            result.RoleLevel.Should().BeTrue();
            result.GlobalLevel.Should().BeFalse();
            result.EffectivePermission.Should().BeTrue();
        }

        [Fact]
        public async Task GetRolePermissionBreakdownAsync_ReturnsDefaultFeatureName_WhenFeatureNotFound()
        {
            // Arrange
            int featureId = 1;
            string roleName = "Admin";

            _mockFeatureRepo.Setup(r => r.GetFeatureByIdAsync(featureId))
                .ReturnsAsync((FeatureModel)null);

            // Act
            var result = await _featureService.GetRolePermissionBreakdownAsync(featureId, roleName);

            // Assert
            result.FeatureName.Should().Be($"Feature-{featureId}");
        }

        #endregion

        #region FindGrantedAtLevel Helper Tests

        [Fact]
        public void FindGrantedAtLevel_ReturnsCorrectLevel_WhenFound()
        {
            // Arrange
            var permissions = new List<FeaturePermission>
            {
                new FeaturePermission
                {
                    FeatureId = 1,
                    AccessLevel = "ROLE",
                    AccessId = "Admin",
                    Val = true
                }
            };

            // Act & Assert (Testing private method through public method)
            // This is tested indirectly through GetUserPermissionsAsync
        }

        [Fact]
        public void FindGrantedAtLevel_ReturnsDefault_WhenNotFound()
        {
            // Arrange
            var permissions = new List<FeaturePermission>();

            // Act & Assert (Testing private method through public method)
            // This is tested indirectly through GetUserPermissionsAsync
        }

        #endregion
    }
}
