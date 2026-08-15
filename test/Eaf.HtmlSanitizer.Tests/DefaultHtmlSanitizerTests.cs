using Abp;
using Abp.Dependency;
using Shouldly;
using Xunit;

namespace Eaf.HtmlSanitizer.Tests
{
    public class DefaultHtmlSanitizerTests
    {
        [Fact]
        public void Dado_HtmlComScript_Quando_Sanitizar_Entao_RemoveScript()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var sanitizer = bootstrapper.IocManager.Resolve<IHtmlSanitizer>();
            var result = sanitizer.Sanitize("<p>safe</p><script>alert(1)</script>");

            result.ShouldNotContain("<script>");
            result.ShouldNotContain("alert");
            result.ShouldContain("safe");
        }

        [Fact]
        public void Dado_HtmlComStyle_Quando_Sanitizar_Entao_RemoveStyle()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var sanitizer = bootstrapper.IocManager.Resolve<IHtmlSanitizer>();
            var result = sanitizer.Sanitize("<style>body{}</style><p>safe</p>");

            result.ShouldNotContain("<style>");
            result.ShouldContain("safe");
        }

        [Fact]
        public void Dado_HtmlComEventHandler_Quando_Sanitizar_Entao_RemoveAtributo()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var sanitizer = bootstrapper.IocManager.Resolve<IHtmlSanitizer>();
            var result = sanitizer.Sanitize("<div onclick=\"alert(1)\" onerror=\"alert(2)\">safe</div>");

            result.ShouldNotContain("onclick");
            result.ShouldNotContain("onerror");
            result.ShouldContain("safe");
        }

        [Fact]
        public void Dado_HtmlComUriJavascript_Quando_Sanitizar_Entao_RemoveEsquema()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var sanitizer = bootstrapper.IocManager.Resolve<IHtmlSanitizer>();
            var result = sanitizer.Sanitize("<a href=\"javascript:alert(1)\">x</a>");

            result.ShouldNotContain("javascript");
        }

        [Fact]
        public void Dado_HtmlSeguro_Quando_Sanitizar_Entao_MantemConteudo()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var sanitizer = bootstrapper.IocManager.Resolve<IHtmlSanitizer>();
            var result = sanitizer.Sanitize("<p><strong>safe</strong></p>");

            result.ShouldContain("<p>");
            result.ShouldContain("<strong>");
            result.ShouldContain("safe");
        }

        [Fact]
        public void Dado_EntradaNula_Quando_Sanitizar_Entao_RetornaVazio()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var sanitizer = bootstrapper.IocManager.Resolve<IHtmlSanitizer>();
            var result = sanitizer.Sanitize(null);

            result.ShouldBe(string.Empty);
        }

        [Fact]
        public void Dado_EntradaVazia_Quando_Sanitizar_Entao_RetornaVazio()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var sanitizer = bootstrapper.IocManager.Resolve<IHtmlSanitizer>();
            var result = sanitizer.Sanitize(string.Empty);

            result.ShouldBe(string.Empty);
        }

        [Fact]
        public void Dado_OpcoesComTagsPermitidas_Quando_Sanitizar_Entao_AplicaRestricao()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var sanitizer = bootstrapper.IocManager.Resolve<IHtmlSanitizer>();
            var options = new EafHtmlSanitizerOptions
            {
                AllowedTags = { "p" }
            };
            var result = sanitizer.Sanitize("<p>keep</p><div>remove</div>", options);

            result.ShouldContain("<p>");
            result.ShouldNotContain("<div>");
        }

        [Fact]
        public void Dado_OpcoesComEsquemasUris_Quando_Sanitizar_Entao_PermiteEsquemaConfigurado()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var sanitizer = bootstrapper.IocManager.Resolve<IHtmlSanitizer>();
            var options = new EafHtmlSanitizerOptions
            {
                AllowedUriSchemes = { "https", "mailto" }
            };
            var result = sanitizer.Sanitize("<a href=\"mailto:test@example.com\">email</a>", options);

            result.ShouldContain("mailto");
            result.ShouldNotContain("javascript");
        }

        [Fact]
        public void Dado_OpcoesComAtributosBloqueados_Quando_Sanitizar_Entao_RemoveAtributosPerigosos()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var sanitizer = bootstrapper.IocManager.Resolve<IHtmlSanitizer>();
            var options = new EafHtmlSanitizerOptions
            {
                AllowedTags = { "div" },
                AllowedAttributes = { "class", "onclick" }
            };
            var result = sanitizer.Sanitize("<div class=\"ok\" onclick=\"alert(1)\">safe</div>", options);

            result.ShouldContain("class");
            result.ShouldNotContain("onclick");
        }

        private static AbpBootstrapper CriarBootstrapper()
        {
            return AbpBootstrapper.Create<EafHtmlSanitizerTestModule>(options =>
            {
                options.IocManager = new IocManager();
            });
        }
    }
}
