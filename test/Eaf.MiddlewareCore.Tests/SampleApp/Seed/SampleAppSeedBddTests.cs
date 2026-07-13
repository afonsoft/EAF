using Eaf.Middleware;
using Eaf.MiddlewareCore.SampleApp.EntityFramework;
using Eaf.MiddlewareCore.SampleApp.EntityFramework.Seed.Host;
using Eaf.MiddlewareCore.SampleApp.EntityFramework.Seed.Tenants;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.SampleApp.Seed
{
    public class SampleAppSeedBddTests : EafMiddlewareTestBase
    {
        [Fact]
        public void Dado_ConfiguracoesDefaultJaCriadas_Quando_DefaultSettingsCreatorCreate_Entao_DeveRetornarSemDuplicar()
        {
            UsingDbContext(context =>
            {
                var creator = new DefaultSettingsCreator(context);
                creator.Create();
                creator.Create();
                creator.ShouldNotBeNull();
            });
        }

        [Fact]
        public void Dado_DefaultTenantJaCriado_Quando_DefaultTenantBuilderCreate_Entao_DeveRetornarSemDuplicar()
        {
            UsingDbContext(context =>
            {
                var builder = new DefaultTenantBuilder(context);
                builder.Create();
                builder.Create();
                builder.ShouldNotBeNull();
            });
        }
    }
}
