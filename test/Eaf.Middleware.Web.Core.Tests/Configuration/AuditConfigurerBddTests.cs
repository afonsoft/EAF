using Abp.Auditing;
using Abp.Configuration.Startup;
using Abp.EntityHistory;
using Eaf.Middleware.Auditing;
using Eaf.Middleware.Web.Configuration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class AuditConfigurerBddTests
    {
        [Fact]
        public void Dado_ConfiguracaoAbp_Quando_Configure_Entao_DeveHabilitarAuditingEEntityHistory()
        {
            // Dado
            var configuration = Substitute.For<IAbpStartupConfiguration>();
            var auditing = Substitute.For<IAuditingConfiguration>();
            var entityHistory = Substitute.For<IEntityHistoryConfiguration>();

            configuration.Auditing.Returns(auditing);
            configuration.EntityHistory.Returns(entityHistory);

            // Quando
            AuditConfigurer.Configure(configuration);

            // Então
            auditing.Received().IsEnabledForAnonymousUsers = false;
            auditing.Received().IsEnabled = true;
            entityHistory.Received().IsEnabled = true;
            entityHistory.Received().IsEnabledForAnonymousUsers = true;
            entityHistory.Received().AddAllAuditedEntities();
        }
    }
}
