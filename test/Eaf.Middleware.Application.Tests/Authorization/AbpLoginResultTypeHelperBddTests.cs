using Abp;
using Abp.Authorization;
using Abp.UI;
using Eaf.Middleware.Authorization;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization
{
    public class AbpLoginResultTypeHelperBddTests
    {
        private readonly AbpLoginResultTypeHelper _sut = new();

        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
        }

        [Theory]
        [InlineData(AbpLoginResultType.InvalidUserNameOrEmailAddress, "LoginFailed", "InvalidUserNameOrPassword")]
        [InlineData(AbpLoginResultType.InvalidPassword, "LoginFailed", "InvalidUserNameOrPassword")]
        [InlineData(AbpLoginResultType.InvalidTenancyName, "LoginFailed", "ThereIsNoTenantDefinedWithNameacme")]
        [InlineData(AbpLoginResultType.TenantIsNotActive, "LoginFailed", "TenantIsNotActive")]
        [InlineData(AbpLoginResultType.UserIsNotActive, "LoginFailed", "UserIsNotActiveAndCanNotLogin")]
        [InlineData(AbpLoginResultType.UserEmailIsNotConfirmed, "LoginFailed", "UserEmailIsNotConfirmedAndCanNotLogin")]
        [InlineData(AbpLoginResultType.LockedOut, "LoginFailed", "UserLockedOutMessage")]
        public void Dado_FalhaDeLogin_Quando_CriarExcecao_Entao_DeveRetornarUserFriendlyException(
            AbpLoginResultType result, string expectedMessage, string expectedDetails)
        {
            var exception = _sut.CreateExceptionForFailedLoginAttempt(result, "john", "acme");

            exception.ShouldNotBeNull();
            exception.ShouldBeOfType<UserFriendlyException>();
            exception.Message.ShouldBe(expectedMessage);
            exception.Details.ShouldBe(expectedDetails);
        }

        [Fact]
        public void Dado_Sucesso_Quando_CriarExcecao_Entao_DeveRetornarMensagemDeErro()
        {
            var exception = _sut.CreateExceptionForFailedLoginAttempt(AbpLoginResultType.Success, "john", "acme");

            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("Don't call this method with a success result!");
        }

        [Theory]
        [InlineData(AbpLoginResultType.InvalidUserNameOrEmailAddress, "InvalidUserNameOrPassword")]
        [InlineData(AbpLoginResultType.InvalidPassword, "InvalidUserNameOrPassword")]
        [InlineData(AbpLoginResultType.InvalidTenancyName, "ThereIsNoTenantDefinedWithNameacme")]
        [InlineData(AbpLoginResultType.TenantIsNotActive, "TenantIsNotActive")]
        [InlineData(AbpLoginResultType.UserIsNotActive, "UserIsNotActiveAndCanNotLogin")]
        [InlineData(AbpLoginResultType.UserEmailIsNotConfirmed, "UserEmailIsNotConfirmedAndCanNotLogin")]
        [InlineData(AbpLoginResultType.LockedOut, "UserLockedOutMessage")]
        public void Dado_FalhaDeLogin_Quando_CriarMensagemLocalizada_Entao_DeveRetornarTextoCorreto(
            AbpLoginResultType result, string expectedMessage)
        {
            var message = _sut.CreateLocalizedMessageForFailedLoginAttempt(result, "john", "acme");

            message.ShouldBe(expectedMessage);
        }

        [Fact]
        public void Dado_Sucesso_Quando_CriarMensagemLocalizada_Entao_DeveLancarAbpException()
        {
            Should.Throw<AbpException>(() =>
                _sut.CreateLocalizedMessageForFailedLoginAttempt(AbpLoginResultType.Success, "john", "acme"));
        }

        [Fact]
        public void Dado_ResultadoDesconhecido_Quando_CriarExcecao_Entao_DeveRetornarMensagemPadrao()
        {
            var unknownResult = (AbpLoginResultType)99;

            var exception = _sut.CreateExceptionForFailedLoginAttempt(unknownResult, "john", "acme");

            exception.Message.ShouldBe("LoginFailed");
        }

        [Fact]
        public void Dado_ResultadoDesconhecido_Quando_CriarMensagemLocalizada_Entao_DeveRetornarMensagemPadrao()
        {
            var unknownResult = (AbpLoginResultType)99;

            var message = _sut.CreateLocalizedMessageForFailedLoginAttempt(unknownResult, "john", "acme");

            message.ShouldBe("LoginFailed");
        }
    }
}
