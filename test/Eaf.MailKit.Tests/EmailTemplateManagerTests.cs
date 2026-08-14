using Abp.UI;
using Eaf.MailKit.Domain;
using Eaf.MailKit.Emailing;
using Shouldly;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MailKit.Tests
{
    public class EmailTemplateManagerTests
    {
        private static EmailTemplateManager CreateManagerWith(params EmailTemplate[] templates)
        {
            var store = new InMemoryEmailTemplateStore();
            foreach (var template in templates)
            {
                store.Add(template);
            }

            return new EmailTemplateManager(store);
        }

        [Fact]
        public async Task Dado_Template_Quando_Renderizar_Com_Modelo_Entao_Substitui_Placeholders()
        {
            var manager = CreateManagerWith(new EmailTemplate
            {
                Name = "Welcome",
                Body = "<h1>Olá, {{Name}}!</h1>"
            });

            var result = await manager.RenderAsync("Welcome", new { Name = "Alice" });

            result.ShouldBe("<h1>Olá, Alice!</h1>");
        }

        [Fact]
        public async Task Dado_Template_Quando_Renderizar_Com_Dicionario_Entao_Substitui_Placeholders()
        {
            var manager = CreateManagerWith(new EmailTemplate
            {
                Name = "Welcome",
                Body = "Olá, {{Name}}!"
            });

            var result = await manager.RenderAsync("Welcome", new Dictionary<string, object> { { "Name", "Bob" } });

            result.ShouldBe("Olá, Bob!");
        }

        [Fact]
        public async Task Dado_TemplateDoTenantAusente_Quando_Renderizar_Entao_Usa_Template_Host()
        {
            var manager = CreateManagerWith(
                new EmailTemplate
                {
                    Name = "Welcome",
                    Body = "Host: {{Name}}",
                    TenantId = null
                });

            var result = await manager.RenderAsync("Welcome", new { Name = "Fallback" }, tenantId: 1);

            result.ShouldBe("Host: Fallback");
        }

        [Fact]
        public async Task Dado_TemplateDoTenantExistente_Quando_Renderizar_Entao_Usa_Template_Do_Tenant()
        {
            var manager = CreateManagerWith(
                new EmailTemplate
                {
                    Name = "Welcome",
                    Body = "Tenant: {{Name}}",
                    TenantId = 1
                });

            var result = await manager.RenderAsync("Welcome", new { Name = "Specific" }, tenantId: 1);

            result.ShouldBe("Tenant: Specific");
        }

        [Fact]
        public async Task Dado_PlaceholderDesconhecido_Quando_Renderizar_Entao_Substitui_Por_Vazio()
        {
            var manager = CreateManagerWith(new EmailTemplate
            {
                Name = "Welcome",
                Body = "Olá, {{Unknown}}!"
            });

            var result = await manager.RenderAsync("Welcome", new { Name = "Alice" });

            result.ShouldBe("Olá, !");
        }

        [Fact]
        public async Task Dado_TemplateInexistente_Quando_Renderizar_Entao_Lanca_UserFriendlyException()
        {
            var manager = CreateManagerWith();

            await Should.ThrowAsync<UserFriendlyException>(async () =>
                await manager.RenderAsync("Missing", new { }));
        }
    }
}
