using Eaf.Middleware.Web.Swagger;
using Microsoft.OpenApi.Models;
using NSubstitute;
using Shouldly;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Swagger
{
    public class SwaggerOperationFilterBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new SwaggerOperationFilter();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_VerificarInterface_Entao_DeveImplementarIOperationFilter()
        {
            var sut = new SwaggerOperationFilter();
            sut.ShouldBeAssignableTo<IOperationFilter>();
        }

        [Fact]
        public void Dado_OperationComParametrosNulos_Quando_Apply_Entao_DeveRetornarSemErro()
        {
            var sut = new SwaggerOperationFilter();
            var operation = new OpenApiOperation();
            var context = Substitute.For<OperationFilterContext>(
                new object[] { null, null, null, null });

            Should.NotThrow(() => sut.Apply(operation, context));
        }

        [Fact]
        public void Dado_OperationComParametroNaoEnum_Quando_Apply_Entao_DeveManterSchemaInalterado()
        {
            var sut = new SwaggerOperationFilter();
            var parameter = new OpenApiParameter { Schema = new OpenApiSchema { Type = "string" } };
            var operation = new OpenApiOperation { Parameters = new List<OpenApiParameter> { parameter } };

            var apiDescription = new Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription { };
            var parameterDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ParameterDescriptor { ParameterType = typeof(string) };
            var parameterDescription = new Microsoft.AspNetCore.Mvc.ApiExplorer.ApiParameterDescription { ParameterDescriptor = parameterDescriptor };
            var list = (System.Collections.IList?)typeof(Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription)
                .GetProperty("ParameterDescriptions")?.GetValue(apiDescription);
            list?.Add(parameterDescription);

            var context = Substitute.For<OperationFilterContext>(
                new object[] { apiDescription, null, null, null });

            sut.Apply(operation, context);

            parameter.Schema.Type.ShouldBe("string");
        }

        [Fact]
        public void Dado_OperationComParametroEnum_Quando_Apply_Entao_DeveSubstituirSchema()
        {
            var sut = new SwaggerOperationFilter();
            var parameter = new OpenApiParameter { Schema = new OpenApiSchema { Type = "string" } };
            var operation = new OpenApiOperation { Parameters = new List<OpenApiParameter> { parameter } };

            var apiDescription = new Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription();
            var parameterDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ParameterDescriptor { ParameterType = typeof(ConsoleColor) };
            var parameterDescription = new Microsoft.AspNetCore.Mvc.ApiExplorer.ApiParameterDescription { ParameterDescriptor = parameterDescriptor };
            var list = (System.Collections.IList?)typeof(Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription)
                .GetProperty("ParameterDescriptions")?.GetValue(apiDescription);
            list?.Add(parameterDescription);

            var schemaGenerator = Substitute.For<ISchemaGenerator>();
            var schemaRepository = new SchemaRepository();
            var context = new OperationFilterContext(apiDescription, schemaGenerator, schemaRepository, null);

            Should.NotThrow(() => sut.Apply(operation, context));
        }
    }
}
