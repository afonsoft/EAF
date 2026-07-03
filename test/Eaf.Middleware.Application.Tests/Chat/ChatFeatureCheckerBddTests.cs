using Abp.Application.Features;
using Eaf.Middleware.Chat;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Chat
{
    public class ChatFeatureCheckerBddTests
    {
        [Fact]
        public void Dado_FeatureChecker_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var featureChecker = Substitute.For<IFeatureChecker>();
            var sut = new ChatFeatureChecker(featureChecker);
            sut.ShouldNotBeNull();
        }
    }
}
