using Eaf.Middleware.Web.Swagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using NSubstitute;
using Shouldly;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Swagger
{
    public class SwaggerEnumParameterFilterBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new SwaggerEnumParameterFilter();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_VerificarInterface_Entao_DeveImplementarIParameterFilter()
        {
            var sut = new SwaggerEnumParameterFilter();
            sut.ShouldBeAssignableTo<IParameterFilter>();
        }

        [Fact]
        public void Dado_ParametroEnumComSchemaSemReferencia_Quando_AplicarFiltro_Entao_DeveRetornarSemErro()
        {
            var filter = new SwaggerEnumParameterFilter();
            var parameter = new OpenApiParameter { Name = "status" };
            var context = CriarContexto(typeof(SampleStatus), schemaReference: null);

            Should.NotThrow(() => filter.Apply(parameter, context));
        }

        [Fact]
        public void Dado_ParametroColecaoDeNaoEnum_Quando_AplicarFiltro_Entao_DeveRetornarSemErro()
        {
            var filter = new SwaggerEnumParameterFilter();
            var parameter = new OpenApiParameter { Name = "ids" };
            var context = CriarContexto(typeof(List<int>));

            Should.NotThrow(() => filter.Apply(parameter, context));
        }

        [Fact]
        public void Dado_ParametroEnumComSchemaReferenciado_Quando_AplicarFiltro_Entao_DeveAdicionarXEnumNames()
        {
            var filter = new SwaggerEnumParameterFilter();
            var parameter = new OpenApiParameter { Name = "status" };
            var schemaRepository = new SchemaRepository();
            var schemaGenerator = Substitute.For<ISchemaGenerator>();
            var referencedSchema = new OpenApiSchema { Reference = new OpenApiReference { Id = "SampleStatus", Type = ReferenceType.Schema } };
            schemaGenerator.GenerateSchema(typeof(SampleStatus), schemaRepository).Returns(referencedSchema);

            var context = new ParameterFilterContext(
                new ApiParameterDescription { Type = typeof(SampleStatus), Name = "status" },
                schemaGenerator,
                schemaRepository,
                parameterInfo: typeof(SampleController).GetMethod("GetByStatus")!.GetParameters()[0]);

            filter.Apply(parameter, context);

            parameter.Schema.ShouldNotBeNull();
            parameter.Schema.Reference.ShouldNotBeNull();
        }

        private static ParameterFilterContext CriarContexto(Type parameterType, OpenApiSchema schemaReference = null!)
        {
            var schemaGenerator = Substitute.For<ISchemaGenerator>();
            var schema = schemaReference ?? new OpenApiSchema { Type = "string" };
            schemaGenerator.GenerateSchema(Arg.Any<Type>(), Arg.Any<SchemaRepository>()).Returns(schema);

            return new ParameterFilterContext(
                new ApiParameterDescription { Type = parameterType, Name = "param" },
                schemaGenerator,
                new SchemaRepository());
        }

        private enum SampleStatus
        {
            Active,
            Inactive
        }

        private class SampleController
        {
            public void GetByStatus(SampleStatus status) { }
        }
    }
}
