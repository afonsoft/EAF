using Eaf.Middleware.Web.Swagger;
using Shouldly;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Swagger
{
    public class SwaggerExtensionsBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(SwaggerExtensions).IsAbstract.ShouldBeTrue();
            typeof(SwaggerExtensions).IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void Dado_SwaggerGenOptions_Quando_InvocarCustomDefaultSchemaIdSelector_Entao_DeveDefinirSchemaId()
        {
            var options = new SwaggerGenOptions();

            options.CustomDefaultSchemaIdSelector();

            options.SchemaGeneratorOptions.SchemaIdSelector.ShouldNotBeNull();
            options.SchemaGeneratorOptions.SchemaIdSelector(typeof(int)).ShouldBe("Int32");
            options.SchemaGeneratorOptions.SchemaIdSelector(typeof(List<string>)).ShouldBe("ListOfString");
            options.SchemaGeneratorOptions.SchemaIdSelector(typeof(Dictionary<string, int>)).ShouldBe("DictionaryOfStringInt32");
        }

        [Fact]
        public void Dado_SwaggerUIOptions_Quando_InjectBaseUrl_Entao_DeveAdicionarScriptComCaminhoBase()
        {
            var options = new SwaggerUIOptions();

            options.InjectBaseUrl("/eaf/api");

            options.HeadContent.ShouldNotBeNull();
            options.HeadContent.ShouldContain("eaf.appPath");
            options.HeadContent.ShouldContain("/eaf/api/");
        }

        [Fact]
        public void Dado_SwaggerUIOptionsComHeadContent_Quando_InjectBaseUrl_Entao_DevePreservarConteudoAnterior()
        {
            var options = new SwaggerUIOptions
            {
                HeadContent = "<!-- existing head content -->"
            };

            options.InjectBaseUrl("/");

            options.HeadContent.ShouldContain("<!-- existing head content -->");
            options.HeadContent.ShouldContain("eaf.appPath");
        }
    }
}
