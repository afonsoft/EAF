using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    public class UserListDtoTests
    {
        [Fact]
        public void Dado_UserListDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new UserListDto();

            dto.AuthenticationSource.ShouldBeNull();
            dto.EmailAddress.ShouldBeNull();
            dto.IsActive.ShouldBeFalse();
            dto.IsEmailConfirmed.ShouldBeFalse();
            dto.LastLoginTime.ShouldBeNull();
            dto.Name.ShouldBeNull();
            dto.ProfilePictureId.ShouldBeNull();
            dto.Roles.ShouldBeNull();
            dto.Surname.ShouldBeNull();
            dto.UserName.ShouldBeNull();
        }

        [Fact]
        public void Dado_UserListDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var now = DateTime.UtcNow;
            var profileId = Guid.NewGuid();
            var dto = new UserListDto
            {
                AuthenticationSource = "LDAP",
                EmailAddress = "admin@test.com",
                IsActive = true,
                IsEmailConfirmed = true,
                LastLoginTime = now,
                Name = "Admin",
                ProfilePictureId = profileId,
                Roles = new List<UserListRoleDto>(),
                Surname = "User",
                UserName = "admin"
            };

            dto.AuthenticationSource.ShouldBe("LDAP");
            dto.EmailAddress.ShouldBe("admin@test.com");
            dto.IsActive.ShouldBeTrue();
            dto.IsEmailConfirmed.ShouldBeTrue();
            dto.LastLoginTime.ShouldBe(now);
            dto.Name.ShouldBe("Admin");
            dto.ProfilePictureId.ShouldBe(profileId);
            dto.Roles.ShouldNotBeNull();
            dto.Surname.ShouldBe("User");
            dto.UserName.ShouldBe("admin");
        }

        [Fact]
        public void Dado_UserListDto_Quando_SemLastModification_Entao_LastModificationDateDeveSerCreationTime()
        {
            var creationTime = new DateTime(2024, 1, 1, 12, 0, 0);
            var dto = new UserListDto();
            dto.CreationTime = creationTime;

            dto.LastModificationDate.ShouldBe(creationTime);
        }

        [Fact]
        public void Dado_UserListDto_Quando_ComLastModification_Entao_LastModificationDateDeveSerLastModificationTime()
        {
            var creationTime = new DateTime(2024, 1, 1, 12, 0, 0);
            var lastModTime = new DateTime(2024, 6, 15, 14, 30, 0);
            var dto = new UserListDto();
            dto.CreationTime = creationTime;
            dto.LastModificationTime = lastModTime;

            dto.LastModificationDate.ShouldBe(lastModTime);
        }
    }
}
