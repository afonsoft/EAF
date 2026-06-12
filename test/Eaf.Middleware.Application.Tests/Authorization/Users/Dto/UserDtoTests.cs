using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Users.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de Usuário seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class UserDtoTests
    {
        #region ChangeUserLanguageDto

        [Fact]
        public void Dado_ChangeUserLanguageDto_Quando_DefinirIdioma_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var dto = new ChangeUserLanguageDto { LanguageName = "pt-BR" };

            // Então
            dto.LanguageName.ShouldBe("pt-BR");
        }

        #endregion

        #region UserListRoleDto

        [Fact]
        public void Dado_UserListRoleDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var dto = new UserListRoleDto
            {
                RoleId = 1,
                RoleName = "Admin"
            };

            // Então
            dto.RoleId.ShouldBe(1);
            dto.RoleName.ShouldBe("Admin");
        }

        #endregion

        #region UserRoleDto

        [Fact]
        public void Dado_UserRoleDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado & Quando
            var dto = new UserRoleDto
            {
                IsAssigned = true,
                RoleDisplayName = "Administrador",
                RoleId = 2,
                RoleName = "Admin"
            };

            // Então
            dto.IsAssigned.ShouldBeTrue();
            dto.RoleDisplayName.ShouldBe("Administrador");
            dto.RoleId.ShouldBe(2);
            dto.RoleName.ShouldBe("Admin");
        }

        #endregion

        #region UserLoginAttemptDto

        [Fact]
        public void Dado_UserLoginAttemptDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            // Dado
            var now = DateTime.UtcNow;

            // Quando
            var dto = new UserLoginAttemptDto
            {
                BrowserInfo = "Chrome 126",
                ClientIpAddress = "192.168.1.1",
                ClientName = "Desktop-PC",
                CreationTime = now,
                Result = "Success",
                TenancyName = "acme",
                UserNameOrEmail = "admin@acme.com"
            };

            // Então
            dto.BrowserInfo.ShouldBe("Chrome 126");
            dto.ClientIpAddress.ShouldBe("192.168.1.1");
            dto.ClientName.ShouldBe("Desktop-PC");
            dto.CreationTime.ShouldBe(now);
            dto.Result.ShouldBe("Success");
            dto.TenancyName.ShouldBe("acme");
            dto.UserNameOrEmail.ShouldBe("admin@acme.com");
        }

        #endregion
    }
}
