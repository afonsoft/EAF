using Eaf.Middleware.Web.Swagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
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
    public class SwaggerOperationFilterBddTests
    {
        [Fact]
        public void Dado_OperacaoComParametrosNulos_Quando_AplicarFiltro_Entao_DeveRetornarSemErro()
        {
            var filter = new SwaggerOperationFilter();
            var operation = new OpenApiOperation();
            var context = CriarContexto(operation, CriarApiDescription(), new List<ApiParameterDescription>());

            Should.NotThrow(() => filter.Apply(operation, context));
        }

        [Fact]
        public void Dado_OperacaoComParametroEnum_Quando_AplicarFiltro_Entao_DeveSubstituirSchema()
        {
            var filter = new SwaggerOperationFilter();
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter { Name = "status" }
                }
            };

            var apiDescription = CriarApiDescription();
            var parameterDescriptor = new ControllerParameterDescriptor
            {
                ParameterInfo = typeof(SampleController).GetMethod("GetByStatus")!.GetParameters()[0],
                ParameterType = typeof(SampleStatus)
            };
            apiDescription.ParameterDescriptions.Add(new ApiParameterDescription
            {
                ParameterDescriptor = parameterDescriptor
            });

            var context = CriarContexto(operation, apiDescription, apiDescription.ParameterDescriptions, typeof(SampleController).GetMethod("GetByStatus")!);

            filter.Apply(operation, context);

            operation.Parameters[0].Schema.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_OperacaoComParametroNaoEnum_Quando_AplicarFiltro_Entao_DeveManterSchema()
        {
            var filter = new SwaggerOperationFilter();
            var schema = new OpenApiSchema();
            var operation = new OpenApiOperation
            {
                Parameters = new List<OpenApiParameter>
                {
                    new OpenApiParameter { Name = "id", Schema = schema }
                }
            };

            var apiDescription = CriarApiDescription();
            var parameterDescriptor = new ControllerParameterDescriptor
            {
                ParameterInfo = typeof(SampleController).GetMethod("GetById")!.GetParameters()[0],
                ParameterType = typeof(int)
            };
            apiDescription.ParameterDescriptions.Add(new ApiParameterDescription
            {
                ParameterDescriptor = parameterDescriptor
            });

            var context = CriarContexto(operation, apiDescription, apiDescription.ParameterDescriptions, typeof(SampleController).GetMethod("GetById")!);

            filter.Apply(operation, context);

            operation.Parameters[0].Schema.ShouldBe(schema);
        }

        private static OperationFilterContext CriarContexto(OpenApiOperation operation, ApiDescription apiDescription, IList<ApiParameterDescription> parameterDescriptions, MethodInfo methodInfo = null!)
        {
            var schemaGenerator = Substitute.For<ISchemaGenerator>();
            schemaGenerator.GenerateSchema(Arg.Any<Type>(), Arg.Any<SchemaRepository>()).Returns(new OpenApiSchema { Type = "string" });

            return new OperationFilterContext(apiDescription, schemaGenerator, new SchemaRepository(), methodInfo ?? typeof(SampleController).GetMethod("GetByStatus")!);
        }

        private static ApiDescription CriarApiDescription()
        {
            return new ApiDescription { RelativePath = "/test" };
        }

        private enum SampleStatus
        {
            Active,
            Inactive
        }

        private class SampleController
        {
            public void GetByStatus(SampleStatus status) { }
            public void GetById(int id) { }
        }
    }
}
