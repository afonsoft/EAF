using Eaf.Middleware.Web.Swagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Shouldly;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Swagger
{
    public class SwaggerFiltersBddTests
    {
        private readonly ISchemaGenerator _schemaGenerator;
        private readonly SchemaRepository _schemaRepository;

        public SwaggerFiltersBddTests()
        {
            _schemaGenerator = new FakeSchemaGenerator();
            _schemaRepository = new SchemaRepository();
        }

        [Fact]
        public void Dado_ParametroNaoNullableComTipoNullable_Quando_AplicarSwaggerNullableParameterFilter_Entao_DeveMarcarComoNullable()
        {
            var parameter = new OpenApiParameter { Schema = new OpenApiSchema { Nullable = false } };
            var context = CriarParameterFilterContext(typeof(int?));

            new SwaggerNullableParameterFilter().Apply(parameter, context);

            parameter.Schema.Nullable.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ParametroJaNullable_Quando_AplicarSwaggerNullableParameterFilter_Entao_DevePermanecerNullable()
        {
            var parameter = new OpenApiParameter { Schema = new OpenApiSchema { Nullable = true } };
            var context = CriarParameterFilterContext(typeof(string));

            new SwaggerNullableParameterFilter().Apply(parameter, context);

            parameter.Schema.Nullable.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ParametroNaoNullableComTipoReferencia_Quando_AplicarSwaggerNullableParameterFilter_Entao_DeveMarcarComoNullable()
        {
            var parameter = new OpenApiParameter { Schema = new OpenApiSchema { Nullable = false } };
            var context = CriarParameterFilterContext(typeof(string));

            new SwaggerNullableParameterFilter().Apply(parameter, context);

            parameter.Schema.Nullable.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ParametroEnum_Quando_AplicarSwaggerEnumParameterFilter_Entao_DeveConfigurarSchemaEnum()
        {
            var parameter = new OpenApiParameter { Schema = new OpenApiSchema() };
            var context = CriarParameterFilterContext(typeof(TestEnum));

            new SwaggerEnumParameterFilter().Apply(parameter, context);

            parameter.Schema.Reference.ShouldNotBeNull();
            parameter.Required.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ColecaoDeEnums_Quando_AplicarSwaggerEnumParameterFilter_Entao_DeveAdicionarSchemaRepositorio()
        {
            var parameter = new OpenApiParameter { Schema = new OpenApiSchema() };
            var context = CriarParameterFilterContext(typeof(List<TestEnum>));

            new SwaggerEnumParameterFilter().Apply(parameter, context);

            _schemaRepository.Schemas.ShouldContainKey("#/definitions/TestEnum");
        }

        [Fact]
        public void Dado_OperacaoComParametroEnum_Quando_AplicarSwaggerOperationFilter_Entao_DeveAtribuirSchemaEnum()
        {
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter> { new OpenApiParameter() }
            };
            var apiDescription = new ApiDescription();
            apiDescription.ParameterDescriptions.Add(new ApiParameterDescription
            {
                ParameterDescriptor = new FakeParameterDescriptor(typeof(TestEnum))
            });
            var method = typeof(SwaggerFiltersBddTests).GetMethod(nameof(Dado_OperacaoComParametroEnum_Quando_AplicarSwaggerOperationFilter_Entao_DeveAtribuirSchemaEnum))!;
            var context = new OperationFilterContext(apiDescription, _schemaGenerator, _schemaRepository, method);

            new SwaggerOperationFilter().Apply(operation, context);

            operation.Parameters.First().Schema.Reference.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_OperacaoSemParametros_Quando_AplicarSwaggerOperationFilter_Entao_NaoDeveFalhar()
        {
            var operation = new OpenApiOperation();
            var apiDescription = new ApiDescription();
            var method = typeof(SwaggerFiltersBddTests).GetMethod(nameof(Dado_OperacaoSemParametros_Quando_AplicarSwaggerOperationFilter_Entao_NaoDeveFalhar))!;
            var context = new OperationFilterContext(apiDescription, _schemaGenerator, _schemaRepository, method);

            new SwaggerOperationFilter().Apply(operation, context);

            operation.Parameters.ShouldBeEmpty();
        }

        [Fact]
        public void Dado_SwaggerUiOptions_Quando_InjectBaseUrl_Entao_DeveAdicionarScriptEafPath()
        {
            var options = new SwaggerUIOptions();

            options.InjectBaseUrl("/api");

            options.HeadContent.ShouldContain("eaf.appPath");
            options.HeadContent.ShouldContain("/api/");
        }

        private ParameterFilterContext CriarParameterFilterContext(Type parameterType)
        {
            var apiParameterDescription = new ApiParameterDescription
            {
                Type = parameterType,
                ParameterDescriptor = new FakeParameterDescriptor(parameterType)
            };
            return new ParameterFilterContext(apiParameterDescription, _schemaGenerator, _schemaRepository, parameterInfo: null);
        }

        private enum TestEnum { ValueOne, ValueTwo }

        private class FakeSchemaGenerator : ISchemaGenerator
        {
            public OpenApiSchema GenerateSchema(Type type, SchemaRepository schemaRepository)
            {
                return new OpenApiSchema
                {
                    Reference = new OpenApiReference { Type = ReferenceType.Schema, Id = type.Name.TrimEnd('?') }
                };
            }

            public bool GeneratePolymorphicSchemas => false;

            public IDictionary<Type, OpenApiSchema> GeneratePolymorphicSchemasBaseMappings => new Dictionary<Type, OpenApiSchema>();

            public OpenApiSchema GenerateSchema(Type type, SchemaRepository schemaRepository, MemberInfo memberInfo, ParameterInfo parameterInfo)
            {
                return GenerateSchema(type, schemaRepository);
            }

            public OpenApiSchema GenerateSchema(Type type, SchemaRepository schemaRepository, MemberInfo memberInfo, ParameterInfo parameterInfo, ApiParameterRouteInfo routeInfo)
            {
                return GenerateSchema(type, schemaRepository);
            }
        }

        private class FakeParameterDescriptor : Microsoft.AspNetCore.Mvc.Abstractions.ParameterDescriptor
        {
            public FakeParameterDescriptor(Type parameterType)
            {
                ParameterType = parameterType;
                Name = "param";
                BindingInfo = new BindingInfo();
            }
        }
    }
}
