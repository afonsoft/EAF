using Eaf.Middleware.Web.Models.TokenAuth;
using Shouldly;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.TokenAuth
{
    /// <summary>
    /// Testes BDD para modelos TokenAuth seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class TokenAuthModelsBddTests
    {
        #region ExternalAuthenticateModel

        [Fact]
        public void Dado_ExternalAuthenticateModel_Quando_DefinirAuthProvider_Entao_DeveArmazenarCorretamente()
        {
            var model = new ExternalAuthenticateModel { AuthProvider = "Google" };
            model.AuthProvider.ShouldBe("Google");
        }

        [Fact]
        public void Dado_ExternalAuthenticateModel_Quando_DefinirProviderKey_Entao_DeveArmazenarCorretamente()
        {
            var model = new ExternalAuthenticateModel { ProviderKey = "key-123" };
            model.ProviderKey.ShouldBe("key-123");
        }

        [Fact]
        public void Dado_ExternalAuthenticateModel_Quando_DefinirProviderAccessCode_Entao_DeveArmazenarCorretamente()
        {
            var model = new ExternalAuthenticateModel { ProviderAccessCode = "access-code" };
            model.ProviderAccessCode.ShouldBe("access-code");
        }

        [Fact]
        public void Dado_ExternalAuthenticateModel_Quando_CamposObrigatoriosVazios_Entao_DeveFalharValidacao()
        {
            var model = new ExternalAuthenticateModel();
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);
            isValid.ShouldBeFalse();
        }

        #endregion

        #region ExternalAuthenticateResultModel

        [Fact]
        public void Dado_ExternalAuthenticateResultModel_Quando_DefinirAccessToken_Entao_DeveArmazenarCorretamente()
        {
            var model = new ExternalAuthenticateResultModel { AccessToken = "ext-token" };
            model.AccessToken.ShouldBe("ext-token");
        }

        [Fact]
        public void Dado_ExternalAuthenticateResultModel_Quando_DefinirWaitingForActivation_Entao_DeveArmazenarCorretamente()
        {
            var model = new ExternalAuthenticateResultModel { WaitingForActivation = true };
            model.WaitingForActivation.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ExternalAuthenticateResultModel_Quando_DefinirUserId_Entao_DeveArmazenarCorretamente()
        {
            var model = new ExternalAuthenticateResultModel { UserId = 42 };
            model.UserId.ShouldBe(42);
        }

        #endregion

        #region ExternalLoginProviderInfoModel

        [Fact]
        public void Dado_ExternalLoginProviderInfoModel_Quando_DefinirName_Entao_DeveArmazenarCorretamente()
        {
            var model = new ExternalLoginProviderInfoModel { Name = "Google" };
            model.Name.ShouldBe("Google");
        }

        [Fact]
        public void Dado_ExternalLoginProviderInfoModel_Quando_DefinirClientId_Entao_DeveArmazenarCorretamente()
        {
            var model = new ExternalLoginProviderInfoModel { ClientId = "client-id-123" };
            model.ClientId.ShouldBe("client-id-123");
        }

        [Fact]
        public void Dado_ExternalLoginProviderInfoModel_Quando_DefinirAdditionalParams_Entao_DeveArmazenarCorretamente()
        {
            var model = new ExternalLoginProviderInfoModel
            {
                AdditionalParams = new Dictionary<string, string> { { "scope", "email" } }
            };
            model.AdditionalParams.ShouldContainKeyAndValue("scope", "email");
        }

        #endregion

        #region ImpersonateModel

        [Fact]
        public void Dado_ImpersonateModel_Quando_DefinirUserId_Entao_DeveArmazenarCorretamente()
        {
            var model = new ImpersonateModel { UserId = 42 };
            model.UserId.ShouldBe(42);
        }

        [Fact]
        public void Dado_ImpersonateModel_Quando_DefinirTenantId_Entao_DeveArmazenarCorretamente()
        {
            var model = new ImpersonateModel { TenantId = 1 };
            model.TenantId.ShouldBe(1);
        }

        [Fact]
        public void Dado_ImpersonateModel_Quando_UserIdZero_Entao_DeveFalharValidacao()
        {
            var model = new ImpersonateModel { UserId = 0 };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);
            isValid.ShouldBeFalse();
        }

        #endregion

        #region ImpersonateResultModel

        [Fact]
        public void Dado_ImpersonateResultModel_Quando_DefinirImpersonationToken_Entao_DeveArmazenarCorretamente()
        {
            var model = new ImpersonateResultModel { ImpersonationToken = "imp-token" };
            model.ImpersonationToken.ShouldBe("imp-token");
        }

        #endregion

        #region ImpersonatedAuthenticateResultModel

        [Fact]
        public void Dado_ImpersonatedAuthenticateResultModel_Quando_DefinirAccessToken_Entao_DeveArmazenarCorretamente()
        {
            var model = new ImpersonatedAuthenticateResultModel { AccessToken = "imp-jwt" };
            model.AccessToken.ShouldBe("imp-jwt");
        }

        [Fact]
        public void Dado_ImpersonatedAuthenticateResultModel_Quando_DefinirExpireInSeconds_Entao_DeveArmazenarCorretamente()
        {
            var model = new ImpersonatedAuthenticateResultModel { ExpireInSeconds = 7200 };
            model.ExpireInSeconds.ShouldBe(7200);
        }

        #endregion

        #region ProviderModel

        [Fact]
        public void Dado_ProviderModel_Quando_DefinirUsernameOrEmailAddress_Entao_DeveArmazenarCorretamente()
        {
            var model = new ProviderModel { UsernameOrEmailAddress = "admin@test.com" };
            model.UsernameOrEmailAddress.ShouldBe("admin@test.com");
        }

        [Fact]
        public void Dado_ProviderModel_Quando_DefinirAuthenticationSource_Entao_DeveArmazenarCorretamente()
        {
            var model = new ProviderModel { AuthenticationSource = "LDAP" };
            model.AuthenticationSource.ShouldBe("LDAP");
        }

        [Fact]
        public void Dado_ProviderModel_Quando_DefinirTenant_Entao_DeveArmazenarCorretamente()
        {
            var tenant = new TenantModal { Id = 1, Name = "Default", TenancyName = "default" };
            var model = new ProviderModel { Tenant = tenant };
            model.Tenant.ShouldNotBeNull();
            model.Tenant.Id.ShouldBe(1);
            model.Tenant.Name.ShouldBe("Default");
            model.Tenant.TenancyName.ShouldBe("default");
        }

        #endregion

        #region SendTwoFactorAuthCodeModel

        [Fact]
        public void Dado_SendTwoFactorAuthCodeModel_Quando_DefinirProvider_Entao_DeveArmazenarCorretamente()
        {
            var model = new SendTwoFactorAuthCodeModel { Provider = "Email" };
            model.Provider.ShouldBe("Email");
        }

        [Fact]
        public void Dado_SendTwoFactorAuthCodeModel_Quando_ProviderVazio_Entao_DeveFalharValidacao()
        {
            var model = new SendTwoFactorAuthCodeModel { UserId = 1 };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, context, results, true);
            isValid.ShouldBeFalse();
        }

        #endregion

        #region SwitchedAccountAuthenticateResultModel

        [Fact]
        public void Dado_SwitchedAccountAuthenticateResultModel_Quando_DefinirAccessToken_Entao_DeveArmazenarCorretamente()
        {
            var model = new SwitchedAccountAuthenticateResultModel { AccessToken = "switched-jwt" };
            model.AccessToken.ShouldBe("switched-jwt");
        }

        [Fact]
        public void Dado_SwitchedAccountAuthenticateResultModel_Quando_DefinirExpireInSeconds_Entao_DeveArmazenarCorretamente()
        {
            var model = new SwitchedAccountAuthenticateResultModel { ExpireInSeconds = 1800 };
            model.ExpireInSeconds.ShouldBe(1800);
        }

        #endregion
    }
}
