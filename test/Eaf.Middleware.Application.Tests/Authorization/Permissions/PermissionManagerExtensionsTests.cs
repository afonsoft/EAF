using Abp.Authorization;
using Abp.Localization;
using Abp.Runtime.Validation;
using Eaf.Middleware.Authorization.Permissions;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Permissions
{
    public class PermissionManagerExtensionsTests
    {
        private readonly IPermissionManager _permissionManager;

        public PermissionManagerExtensionsTests()
        {
            _permissionManager = Substitute.For<IPermissionManager>();
        }

        [Fact]
        public void GetPermissionsFromNamesByValidating_AllPermissionsExist_ReturnsPermissions()
        {
            // Arrange
            var permissionNames = new[] { "Permission1", "Permission2", "Permission3" };
            var permission1 = new Permission("Permission1", new LocalizableString("Permission 1", "Eaf"));
            var permission2 = new Permission("Permission2", new LocalizableString("Permission 2", "Eaf"));
            var permission3 = new Permission("Permission3", new LocalizableString("Permission 3", "Eaf"));

            _permissionManager.GetPermissionOrNull("Permission1").Returns(permission1);
            _permissionManager.GetPermissionOrNull("Permission2").Returns(permission2);
            _permissionManager.GetPermissionOrNull("Permission3").Returns(permission3);

            // Act
            var result = _permissionManager.GetPermissionsFromNamesByValidating(permissionNames);

            // Assert
            result.Count().ShouldBe(3);
            result.ShouldContain(permission1);
            result.ShouldContain(permission2);
            result.ShouldContain(permission3);
        }

        [Fact]
        public void GetPermissionsFromNamesByValidating_SomePermissionsDoNotExist_ThrowsException()
        {
            // Arrange
            var permissionNames = new[] { "Permission1", "InvalidPermission", "Permission2" };
            var permission1 = new Permission("Permission1", new LocalizableString("Permission 1", "Eaf"));
            var permission2 = new Permission("Permission2", new LocalizableString("Permission 2", "Eaf"));

            _permissionManager.GetPermissionOrNull("Permission1").Returns(permission1);
            _permissionManager.GetPermissionOrNull("InvalidPermission").Returns((Permission)null!);
            _permissionManager.GetPermissionOrNull("Permission2").Returns(permission2);

            // Act & Assert
            var exception = Should.Throw<AbpValidationException>(() =>
            {
                _permissionManager.GetPermissionsFromNamesByValidating(permissionNames);
            });

            exception.Message.ShouldContain("1 undefined permission");
            exception.ValidationErrors.Count.ShouldBe(1);
            exception.ValidationErrors.First().ErrorMessage!.ShouldContain("InvalidPermission");
        }

        [Fact]
        public void GetPermissionsFromNamesByValidating_AllPermissionsDoNotExist_ThrowsException()
        {
            // Arrange
            var permissionNames = new[] { "InvalidPermission1", "InvalidPermission2", "InvalidPermission3" };

            _permissionManager.GetPermissionOrNull("InvalidPermission1").Returns((Permission)null!);
            _permissionManager.GetPermissionOrNull("InvalidPermission2").Returns((Permission)null!);
            _permissionManager.GetPermissionOrNull("InvalidPermission3").Returns((Permission)null!);

            // Act & Assert
            var exception = Should.Throw<AbpValidationException>(() =>
            {
                _permissionManager.GetPermissionsFromNamesByValidating(permissionNames);
            });

            exception.Message.ShouldContain("3 undefined permission");
            exception.ValidationErrors.Count.ShouldBe(3);
        }

        [Fact]
        public void GetPermissionsFromNamesByValidating_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var permissionNames = Array.Empty<string>();

            // Act
            var result = _permissionManager.GetPermissionsFromNamesByValidating(permissionNames);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(0);
        }

        [Fact]
        public void GetPermissionsFromNamesByValidating_SinglePermissionExists_ReturnsPermission()
        {
            // Arrange
            var permissionNames = new[] { "Permission1" };
            var permission1 = new Permission("Permission1", new LocalizableString("Permission 1", "Eaf"));

            _permissionManager.GetPermissionOrNull("Permission1").Returns(permission1);

            // Act
            var result = _permissionManager.GetPermissionsFromNamesByValidating(permissionNames);

            // Assert
            result.Count().ShouldBe(1);
            result.First().ShouldBe(permission1);
        }

        [Fact]
        public void GetPermissionsFromNamesByValidating_SinglePermissionDoesNotExist_ThrowsException()
        {
            // Arrange
            var permissionNames = new[] { "InvalidPermission" };

            _permissionManager.GetPermissionOrNull("InvalidPermission").Returns((Permission)null!);

            // Act & Assert
            var exception = Should.Throw<AbpValidationException>(() =>
            {
                _permissionManager.GetPermissionsFromNamesByValidating(permissionNames);
            });

            exception.Message.ShouldContain("1 undefined permission");
            exception.ValidationErrors.Count.ShouldBe(1);
        }

        [Fact]
        public void GetPermissionsFromNamesByValidating_NullPermissionNames_ThrowsNullReferenceException()
        {
            // Arrange
            IEnumerable<string> permissionNames = null!;

            // Act & Assert
            Should.Throw<NullReferenceException>(() =>
            {
                _permissionManager.GetPermissionsFromNamesByValidating(permissionNames);
            });
        }
    }
}
