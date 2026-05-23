using Eaf.Middleware.Authorization.Permissions.Dto;
using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    public class UsersDtoCoverageTests
    {
        [Fact]
        public void ChangeUserLanguageDto_ShouldSet()
        {
            var dto = new ChangeUserLanguageDto { LanguageName = "en" };
            dto.LanguageName.ShouldBe("en");
        }

        [Fact]
        public void CreateActiveDirectoryUserInput_ShouldSet()
        {
            var dto = new CreateActiveDirectoryUserInput
            {
                AssignedRoleNames = new[] { "Admin" },
                IsActive = true,
                UserNames = new[] { "alice", "bob" }
            };
            dto.AssignedRoleNames.Length.ShouldBe(1);
            dto.UserNames.Length.ShouldBe(2);
            dto.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void CreateLdapUserInput_ShouldSet()
        {
            var dto = new CreateLdapUserInput
            {
                AssignedRoleNames = new[] { "User" },
                IsActive = false,
                UserNames = new[] { "carol" }
            };
            dto.AssignedRoleNames.Length.ShouldBe(1);
            dto.IsActive.ShouldBeFalse();
            dto.UserNames.Length.ShouldBe(1);
        }

        [Fact]
        public void CreateOrUpdateUserInput_ShouldSet()
        {
            var dto = new CreateOrUpdateUserInput
            {
                AssignedRoleNames = new[] { "Admin" },
                SendActivationEmail = true,
                SetRandomPassword = true,
                User = new UserEditDto
                {
                    EmailAddress = "a@b.com",
                    Name = "n",
                    Surname = "s",
                    UserName = "u"
                }
            };
            dto.SendActivationEmail.ShouldBeTrue();
            dto.SetRandomPassword.ShouldBeTrue();
            dto.User.UserName.ShouldBe("u");
        }

        [Fact]
        public void GetUserForEditOutput_ShouldSet()
        {
            var pic = Guid.NewGuid();
            var dto = new GetUserForEditOutput
            {
                ProfilePictureId = pic,
                Roles = new[] { new UserRoleDto { IsAssigned = true, RoleId = 1, RoleName = "R" } },
                User = new UserEditDto()
            };
            dto.ProfilePictureId.ShouldBe(pic);
            dto.Roles.Length.ShouldBe(1);
            dto.User.ShouldNotBeNull();
        }

        [Fact]
        public void GetUserPermissionsForEditOutput_ShouldSet()
        {
            var dto = new GetUserPermissionsForEditOutput
            {
                GrantedPermissionNames = new List<string> { "A" },
                Permissions = new List<FlatPermissionDto> { new() { Name = "A" } }
            };
            dto.GrantedPermissionNames.Count.ShouldBe(1);
            dto.Permissions.Count.ShouldBe(1);
        }

        [Fact]
        public void GetUsersInput_Normalize_DefaultsSorting()
        {
            var dto = new GetUsersInput();
            dto.Filter.ShouldBe("");
            dto.Normalize();
            dto.Sorting.ShouldBe("Name,Surname");
        }

        [Fact]
        public void GetUsersInput_Normalize_KeepsExistingSorting()
        {
            var dto = new GetUsersInput { Sorting = "UserName" };
            dto.Normalize();
            dto.Sorting.ShouldBe("UserName");
        }

        [Fact]
        public void UpdateUserPermissionsInput_ShouldSet()
        {
            var dto = new UpdateUserPermissionsInput
            {
                GrantedPermissionNames = new List<string> { "A" },
                Id = 5
            };
            dto.GrantedPermissionNames.Count.ShouldBe(1);
            dto.Id.ShouldBe(5);
        }

        [Fact]
        public void UserEditDto_ShouldSetAll()
        {
            var dto = new UserEditDto
            {
                EmailAddress = "a@b.com",
                Id = 1,
                IsActive = true,
                IsLockoutEnabled = false,
                Name = "n",
                Password = "p",
                ShouldChangePasswordOnNextLogin = true,
                Surname = "s",
                UserName = "u",
                PhoneNumber = "123"
            };
            dto.EmailAddress.ShouldBe("a@b.com");
            dto.Id.ShouldBe(1);
            dto.IsActive.ShouldBeTrue();
            dto.IsLockoutEnabled.ShouldBeFalse();
            dto.Name.ShouldBe("n");
            dto.Password.ShouldBe("p");
            dto.ShouldChangePasswordOnNextLogin.ShouldBeTrue();
            dto.Surname.ShouldBe("s");
            dto.UserName.ShouldBe("u");
            dto.PhoneNumber.ShouldBe("123");
        }

        [Fact]
        public void UserListDto_LastModificationDate_FallbackAndValue()
        {
            var dto = new UserListDto
            {
                UserName = "u",
                CreationTime = new DateTime(2020, 1, 1),
                LastModificationTime = null
            };
            dto.LastModificationDate.ShouldBe(new DateTime(2020, 1, 1));

            dto.LastModificationTime = new DateTime(2023, 3, 3);
            dto.LastModificationDate.ShouldBe(new DateTime(2023, 3, 3));
        }

        [Fact]
        public void UserListDto_ShouldSetAll()
        {
            var dto = new UserListDto
            {
                AuthenticationSource = "src",
                EmailAddress = "a@b.com",
                IsActive = true,
                IsEmailConfirmed = true,
                LastLoginTime = new DateTime(2023, 1, 1),
                Name = "n",
                ProfilePictureId = Guid.NewGuid(),
                Roles = new List<UserListRoleDto> { new() { RoleId = 1, RoleName = "R" } },
                Surname = "s",
                UserName = "u"
            };
            dto.AuthenticationSource.ShouldBe("src");
            dto.EmailAddress.ShouldBe("a@b.com");
            dto.IsActive.ShouldBeTrue();
            dto.IsEmailConfirmed.ShouldBeTrue();
            dto.LastLoginTime.ShouldNotBeNull();
            dto.Name.ShouldBe("n");
            dto.ProfilePictureId.ShouldNotBeNull();
            dto.Roles.Count.ShouldBe(1);
            dto.Surname.ShouldBe("s");
            dto.UserName.ShouldBe("u");
        }

        [Fact]
        public void UserListRoleDto_ShouldSet()
        {
            var dto = new UserListRoleDto { RoleId = 1, RoleName = "R" };
            dto.RoleId.ShouldBe(1);
            dto.RoleName.ShouldBe("R");
        }

        [Fact]
        public void UserLoginAttemptDto_ShouldSet()
        {
            var now = DateTime.UtcNow;
            var dto = new UserLoginAttemptDto
            {
                BrowserInfo = "b",
                ClientIpAddress = "ip",
                ClientName = "c",
                CreationTime = now,
                Result = "ok",
                TenancyName = "tn",
                UserNameOrEmail = "u"
            };
            dto.BrowserInfo.ShouldBe("b");
            dto.ClientIpAddress.ShouldBe("ip");
            dto.ClientName.ShouldBe("c");
            dto.CreationTime.ShouldBe(now);
            dto.Result.ShouldBe("ok");
            dto.TenancyName.ShouldBe("tn");
            dto.UserNameOrEmail.ShouldBe("u");
        }

        [Fact]
        public void UserRoleDto_ShouldSet()
        {
            var dto = new UserRoleDto { IsAssigned = true, RoleDisplayName = "Admin", RoleId = 1, RoleName = "Admin" };
            dto.IsAssigned.ShouldBeTrue();
            dto.RoleDisplayName.ShouldBe("Admin");
            dto.RoleId.ShouldBe(1);
            dto.RoleName.ShouldBe("Admin");
        }
    }
}
