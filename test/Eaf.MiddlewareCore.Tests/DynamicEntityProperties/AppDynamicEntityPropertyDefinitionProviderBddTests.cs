using Abp.DynamicEntityProperties;
using Eaf.Middleware.Core.DynamicEntityProperties;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.DynamicEntityProperties
{
    /// <summary>
    /// Testes BDD para AppDynamicEntityPropertyDefinitionProvider seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class AppDynamicEntityPropertyDefinitionProviderBddTests
    {
        [Fact]
        public void Dado_ConstrutorPadrao_Quando_Criar_Entao_DeveHerdarDynamicEntityPropertyDefinitionProvider()
        {
            var provider = new AppDynamicEntityPropertyDefinitionProvider();

            provider.ShouldBeAssignableTo<DynamicEntityPropertyDefinitionProvider>();
        }

        [Fact]
        public void Dado_Contexto_Quando_SetDynamicEntityProperties_Entao_DeveRegistrarSemErros()
        {
            var provider = new AppDynamicEntityPropertyDefinitionProvider();
            var context = Substitute.For<IDynamicEntityPropertyDefinitionContext>();
            context.Manager.Returns(Substitute.For<IDynamicEntityPropertyDefinitionManager>());

            Should.NotThrow(() => provider.SetDynamicEntityProperties(context));
        }
    }
}
