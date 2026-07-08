using Abp.Application.Features;
using Abp.UI;
using Eaf.Middleware.Chat;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Chat
{
    public class ChatFeatureCheckerBddTests
    {
        private readonly IFeatureChecker _featureChecker;
        private readonly ChatFeatureChecker _sut;

        public ChatFeatureCheckerBddTests()
        {
            _featureChecker = Substitute.For<IFeatureChecker>();
            _sut = new ChatFeatureChecker(_featureChecker);
        }

        [Fact]
        public void Dado_FeatureChecker_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_GrupoHabilitado_Quando_CheckChatGroupFeature_Entao_DeveRetornarVerdadeiro()
        {
            _featureChecker.IsEnabled(1, AppFeatures.GroupChatFeature).Returns(true);
            _sut.CheckChatGroupFeature(1).ShouldBeTrue();
        }

        [Fact]
        public void Dado_GrupoDesabilitado_Quando_CheckChatGroupFeature_Entao_DeveRetornarFalso()
        {
            _featureChecker.IsEnabled(1, AppFeatures.GroupChatFeature).Returns(false);
            _sut.CheckChatGroupFeature(1).ShouldBeFalse();
        }

        [Fact]
        public void Dado_TenantParaOMesmoTenant_Quando_CheckChatFeatures_Entao_NaoDeveLancarExcecao()
        {
            _featureChecker.IsEnabled(1, AppFeatures.ChatFeature).Returns(true);
            _sut.CheckChatFeatures(1, 1);
        }

        [Fact]
        public void Dado_TenantParaOutroTenantComRecursosHabilitados_Quando_CheckChatFeatures_Entao_NaoDeveLancarExcecao()
        {
            _featureChecker.IsEnabled(Arg.Any<int>(), AppFeatures.ChatFeature).Returns(true);
            _featureChecker.IsEnabled(Arg.Any<int>(), AppFeatures.TenantToTenantChatFeature).Returns(true);
            _sut.CheckChatFeatures(1, 2);
        }

        [Fact]
        public void Dado_TenantParaOutroTenantComTenantToTenantDesabilitado_Quando_CheckChatFeatures_Entao_DeveLancarUserFriendlyException()
        {
            _featureChecker.IsEnabled(Arg.Any<int>(), AppFeatures.ChatFeature).Returns(true);
            _featureChecker.IsEnabled(Arg.Any<int>(), AppFeatures.TenantToHostChatFeature).Returns(true);
            _featureChecker.IsEnabled(1, AppFeatures.TenantToTenantChatFeature).Returns(false);

            Should.Throw<UserFriendlyException>(() => _sut.CheckChatFeatures(1, 2));
        }

        [Fact]
        public void Dado_TenantParaHostComRecursosHabilitados_Quando_CheckChatFeatures_Entao_NaoDeveLancarExcecao()
        {
            _featureChecker.IsEnabled(1, AppFeatures.ChatFeature).Returns(true);
            _featureChecker.IsEnabled(1, AppFeatures.TenantToHostChatFeature).Returns(true);
            _sut.CheckChatFeatures(1, null);
        }

        [Fact]
        public void Dado_TenantParaHostComTenantToHostDesabilitado_Quando_CheckChatFeatures_Entao_DeveLancarUserFriendlyException()
        {
            _featureChecker.IsEnabled(1, AppFeatures.ChatFeature).Returns(true);
            _featureChecker.IsEnabled(1, AppFeatures.TenantToHostChatFeature).Returns(false);

            Should.Throw<UserFriendlyException>(() => _sut.CheckChatFeatures(1, null));
        }
    }
}
