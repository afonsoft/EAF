using Eaf.Middleware.Sessions.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Sessions.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de Sessions seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class SessionDtoBddTests
    {
        #region ApplicationInfoDto

        [Fact]
        public void Dado_ApplicationInfoDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ApplicationInfoDto
            {
                Version = "10.0.0",
                ReleaseDate = new DateTime(2026, 1, 1),
                Currency = "BRL",
                CurrencySign = "R$",
                TwoFactorCodeExpireSeconds = 300,
                Features = new Dictionary<string, bool>
                {
                    { "Chat", true },
                    { "Notifications", false }
                }
            };

            dto.Version.ShouldBe("10.0.0");
            dto.ReleaseDate.ShouldBe(new DateTime(2026, 1, 1));
            dto.Currency.ShouldBe("BRL");
            dto.CurrencySign.ShouldBe("R$");
            dto.TwoFactorCodeExpireSeconds.ShouldBe(300);
            dto.Features.Count.ShouldBe(2);
        }

        #endregion

        #region TenantLoginInfoDto

        [Fact]
        public void Dado_TenantLoginInfoDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new TenantLoginInfoDto
            {
                Id = 1,
                TenancyName = "acme",
                Name = "Acme Corp",
                CreationTime = new DateTime(2025, 6, 1)
            };

            dto.Id.ShouldBe(1);
            dto.TenancyName.ShouldBe("acme");
            dto.Name.ShouldBe("Acme Corp");
            dto.CreationTime.Year.ShouldBe(2025);
        }

        #endregion

        #region UserLoginInfoDto

        [Fact]
        public void Dado_UserLoginInfoDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new UserLoginInfoDto
            {
                Name = "João",
                Surname = "Silva",
                UserName = "joao.silva",
                EmailAddress = "joao@acme.com"
            };

            dto.Name.ShouldBe("João");
            dto.Surname.ShouldBe("Silva");
            dto.UserName.ShouldBe("joao.silva");
            dto.EmailAddress.ShouldBe("joao@acme.com");
        }

        #endregion

        #region GetCurrentLoginInformationsOutput

        [Fact]
        public void Dado_GetCurrentLoginInformationsOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto { Version = "1.0" },
                Tenant = new TenantLoginInfoDto { TenancyName = "acme" },
                User = new UserLoginInfoDto { UserName = "admin" }
            };

            output.Application.ShouldNotBeNull();
            output.Tenant.ShouldNotBeNull();
            output.User.ShouldNotBeNull();
        }

        #endregion

        #region UpdateUserSignInTokenOutput

        [Fact]
        public void Dado_UpdateUserSignInTokenOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new UpdateUserSignInTokenOutput
            {
                EncodedTenantId = "encoded-tenant-id",
                EncodedUserId = "encoded-user-id",
                SignInToken = "sign-in-token-123"
            };

            dto.EncodedTenantId.ShouldBe("encoded-tenant-id");
            dto.EncodedUserId.ShouldBe("encoded-user-id");
            dto.SignInToken.ShouldBe("sign-in-token-123");
        }

        #endregion
    }
}
