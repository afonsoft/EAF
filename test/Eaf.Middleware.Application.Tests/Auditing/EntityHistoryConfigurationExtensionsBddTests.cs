using Abp;
using Abp.EntityHistory;
using Eaf.Middleware.Auditing;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Tests.Auditing
{
    public class EntityHistoryConfigurationExtensionsBddTests
    {
        private static IEntityHistorySelectorList CriarListaSeletores()
        {
            var list = new List<NamedTypeSelector>();
            var selectors = Substitute.For<IEntityHistorySelectorList>();
            selectors.When(x => x.Add(Arg.Any<NamedTypeSelector>())).Do(x => list.Add(x.Arg<NamedTypeSelector>()));
            selectors.GetEnumerator().Returns(callInfo => list.GetEnumerator());
            selectors[Arg.Any<int>()].Returns(callInfo => list[callInfo.Arg<int>()]);
            selectors.Count.Returns(callInfo => list.Count);
            return selectors;
        }

        [Fact]
        public void Dado_EntityHistoryHabilitadoSemSeletor_Quando_AddAllAuditedEntities_Entao_DeveAdicionarSeletorDeTodasEntidades()
        {
            var selectors = CriarListaSeletores();

            var configuration = Substitute.For<IEntityHistoryConfiguration>();
            configuration.IsEnabled.Returns(true);
            configuration.Selectors.Returns(selectors);

            configuration.AddAllAuditedEntities();

            configuration.Selectors.Count.ShouldBe(1);
            configuration.Selectors.First().Name.ShouldBe(EntityHistoryConfigurationExtensions.AllEntitiesSelectorName);
        }

        [Fact]
        public void Dado_EntityHistoryHabilitadoComSeletorExistente_Quando_AddAllAuditedEntities_Entao_DeveRetornarSemAdicionar()
        {
            var selectors = CriarListaSeletores();
            selectors.Add(new NamedTypeSelector(EntityHistoryConfigurationExtensions.AllEntitiesSelectorName, _ => true));

            var configuration = Substitute.For<IEntityHistoryConfiguration>();
            configuration.IsEnabled.Returns(true);
            configuration.Selectors.Returns(selectors);

            configuration.AddAllAuditedEntities();

            configuration.Selectors.Count.ShouldBe(1);
        }

        [Fact]
        public void Dado_EntityHistoryDesabilitado_Quando_AddAllAuditedEntities_Entao_DeveRetornarSemAdicionar()
        {
            var selectors = CriarListaSeletores();

            var configuration = Substitute.For<IEntityHistoryConfiguration>();
            configuration.IsEnabled.Returns(false);
            configuration.Selectors.Returns(selectors);

            configuration.AddAllAuditedEntities();

            configuration.Selectors.Count.ShouldBe(0);
        }
    }
}
