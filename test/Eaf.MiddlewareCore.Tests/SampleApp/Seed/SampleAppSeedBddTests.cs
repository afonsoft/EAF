using Eaf.Middleware;
using Eaf.MiddlewareCore.SampleApp.EntityFramework;
using Eaf.MiddlewareCore.SampleApp.EntityFramework.Seed.Host;
using Eaf.MiddlewareCore.SampleApp.EntityFramework.Seed.Tenants;
using Shouldly;
using System.Linq;
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

        [Fact]
        public void Dado_IdiomasIniciais_Quando_DefaultLanguagesCreatorCreate_Entao_DeveAdicionarIdiomasSemDuplicar()
        {
            UsingDbContext(context =>
            {
                context.Languages.Add(new Abp.Localization.ApplicationLanguage(null, "en", "English", "famfamfam-flags gb"));
                context.SaveChanges();

                var creator = new DefaultLanguagesCreator(context);
                creator.Create();
                creator.Create();

                creator.ShouldNotBeNull();
                context.Languages.Any(l => l.Name == "tr").ShouldBeTrue();
            });
        }

        [Fact]
        public void Dado_Tenant1JaCriado_Quando_TenantRoleAndUserBuilderCreate_Entao_DeveRetornarSemDuplicar()
        {
            UsingDbContext(context =>
            {
                var builder = new TenantRoleAndUserBuilder(context, 1);
                builder.Create();
                builder.Create();
                builder.ShouldNotBeNull();
            });
        }

        [Fact]
        public void Dado_Tenant2NaoCriado_Quando_TenantRoleAndUserBuilderCreate_Entao_DeveCriarRolesEUsuariosSemUserAccount()
        {
            UsingDbContext(context =>
            {
                var builder = new TenantRoleAndUserBuilder(context, 2);
                builder.Create();
                builder.Create();

                builder.ShouldNotBeNull();
                context.Users.Any(u => u.TenantId == 2).ShouldBeTrue();
            });
        }
    }
}
