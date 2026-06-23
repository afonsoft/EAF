using Castle.Core.Logging;
using Eaf.Middleware.Core.Authentication.External;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.External.Providers
{
    /// <summary>
    /// Testes BDD para ExternalAuthProviderApiBase seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class ExternalAuthProviderApiBaseBddTests
    {
        private sealed class TestableExternalAuthProviderApi : ExternalAuthProviderApiBase
        {
            private readonly ExternalAuthUserInfo _userInfo;

            public TestableExternalAuthProviderApi(ExternalAuthUserInfo userInfo = null)
            {
                _userInfo = userInfo;
            }

            public override Task<ExternalAuthUserInfo> GetUserInfo(string accessCode)
            {
                return Task.FromResult(_userInfo ?? new ExternalAuthUserInfo
                {
                    ProviderKey = "test-key-" + accessCode,
                    Provider = "Test",
                    Name = "Test User",
                    EmailAddress = "test@example.com"
                });
            }
        }

        #region Instanciacao

        [Fact]
        public void Dado_Padrao_Quando_CriarInstancia_Entao_LoggerDeveSerNullLogger()
        {
            var sut = new TestableExternalAuthProviderApi();
            sut.Logger.ShouldBe(NullLogger.Instance);
        }

        [Fact]
        public void Dado_Padrao_Quando_CriarInstancia_Entao_ProviderInfoDeveSerNull()
        {
            var sut = new TestableExternalAuthProviderApi();
            sut.ProviderInfo.ShouldBeNull();
        }

        #endregion

        #region Initialize

        [Fact]
        public void Dado_ProviderInfo_Quando_Initialize_Entao_DeveAtribuirProviderInfo()
        {
            // Dado
            var sut = new TestableExternalAuthProviderApi();
            var providerInfo = new ExternalLoginProviderInfo("Test", "clientId", "secret", null, typeof(TestableExternalAuthProviderApi));

            // Quando
            sut.Initialize(providerInfo);

            // Entao
            sut.ProviderInfo.ShouldBe(providerInfo);
        }

        #endregion

        #region IsValidUser

        [Fact]
        public async Task Dado_UserIdCorreto_Quando_IsValidUser_Entao_DeveRetornarTrue()
        {
            // Dado
            var userInfo = new ExternalAuthUserInfo { ProviderKey = "test-key-abc123" };
            var sut = new TestableExternalAuthProviderApi(userInfo);

            // Quando
            var result = await sut.IsValidUser("test-key-abc123", "abc123");

            // Entao
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_UserIdIncorreto_Quando_IsValidUser_Entao_DeveRetornarFalse()
        {
            // Dado
            var userInfo = new ExternalAuthUserInfo { ProviderKey = "correct-key" };
            var sut = new TestableExternalAuthProviderApi(userInfo);

            // Quando
            var result = await sut.IsValidUser("wrong-key", "some-access-code");

            // Entao
            result.ShouldBeFalse();
        }

        #endregion

        #region Logger

        [Fact]
        public void Dado_LoggerCustom_Quando_DefinirLogger_Entao_DeveArmazenar()
        {
            var sut = new TestableExternalAuthProviderApi();
            var customLogger = Substitute.For<ILogger>();
            sut.Logger = customLogger;
            sut.Logger.ShouldBe(customLogger);
        }

        #endregion
    }
}
