using Eaf.Middleware.Swagger;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using NSubstitute;
using Shouldly;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Swagger
{
    /// <summary>
    /// Testes BDD para SwaggerOperationIdFilter seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class SwaggerOperationIdFilterBddTests
    {
        private readonly SwaggerOperationIdFilter _sut;

        public SwaggerOperationIdFilterBddTests()
        {
            _sut = new SwaggerOperationIdFilter();
        }

        #region Apply

        [Fact]
        public void Dado_ApiDescription_Quando_Apply_Entao_DeveDefinirOperationId()
        {
            // Dado
            var operation = new OpenApiOperation();
            var apiDescription = new ApiDescription
            {
                RelativePath = "api/services/app/User/GetAll",
                HttpMethod = "GET"
            };

            var generator = new SchemaGenerator(new SchemaGeneratorOptions(), new JsonSerializerDataContractResolver(new System.Text.Json.JsonSerializerOptions()));
            var schemaRepo = new SchemaRepository();
            var methodInfo = typeof(object).GetMethod("ToString");
            var context = new OperationFilterContext(apiDescription, generator, schemaRepo, methodInfo);

            // Quando
            _sut.Apply(operation, context);

            // Entao
            operation.OperationId.ShouldNotBeNullOrEmpty();
            operation.OperationId.ShouldContain("Get");
        }

        [Fact]
        public void Dado_ApiDescriptionComPathParameters_Quando_Apply_Entao_DeveConterByNoId()
        {
            // Dado
            var operation = new OpenApiOperation();
            var apiDescription = new ApiDescription
            {
                RelativePath = "api/services/app/User/{id}",
                HttpMethod = "GET"
            };

            var generator = new SchemaGenerator(new SchemaGeneratorOptions(), new JsonSerializerDataContractResolver(new System.Text.Json.JsonSerializerOptions()));
            var schemaRepo = new SchemaRepository();
            var methodInfo = typeof(object).GetMethod("ToString");
            var context = new OperationFilterContext(apiDescription, generator, schemaRepo, methodInfo);

            // Quando
            _sut.Apply(operation, context);

            // Entao
            operation.OperationId.ShouldContain("ById");
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_SwaggerOperationIdFilter_Quando_CriarInstancia_Entao_DeveSerIOperationFilter()
        {
            _sut.ShouldNotBeNull();
            _sut.ShouldBeAssignableTo<Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter>();
        }

        #endregion
    }
}
