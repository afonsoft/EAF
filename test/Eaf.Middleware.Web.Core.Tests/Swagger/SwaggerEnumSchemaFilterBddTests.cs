using Eaf.Middleware.Web.Swagger;
using Microsoft.OpenApi.Models;
using Shouldly;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Swagger
{
    /// <summary>
    /// Testes BDD para SwaggerEnumSchemaFilter seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class SwaggerEnumSchemaFilterBddTests
    {
        private enum TestEnum { Value1, Value2, Value3 }

        private readonly SwaggerEnumSchemaFilter _sut;

        public SwaggerEnumSchemaFilterBddTests()
        {
            _sut = new SwaggerEnumSchemaFilter();
        }

        #region Apply

        [Fact]
        public void Dado_TipoEnum_Quando_Apply_Entao_DeveAdicionarXEnumNames()
        {
            // Dado
            var schema = new OpenApiSchema();
            var generator = new SchemaGenerator(new SchemaGeneratorOptions(), new JsonSerializerDataContractResolver(new System.Text.Json.JsonSerializerOptions()));
            var schemaRepo = new SchemaRepository();
            var context = new SchemaFilterContext(typeof(TestEnum), generator, schemaRepo);

            // Quando
            _sut.Apply(schema, context);

            // Entao
            schema.Extensions.ContainsKey("x-enumNames").ShouldBeTrue();
        }

        [Fact]
        public void Dado_TipoNaoEnum_Quando_Apply_Entao_NaoDeveAdicionarXEnumNames()
        {
            // Dado
            var schema = new OpenApiSchema();
            var generator = new SchemaGenerator(new SchemaGeneratorOptions(), new JsonSerializerDataContractResolver(new System.Text.Json.JsonSerializerOptions()));
            var schemaRepo = new SchemaRepository();
            var context = new SchemaFilterContext(typeof(string), generator, schemaRepo);

            // Quando
            _sut.Apply(schema, context);

            // Entao
            schema.Extensions.ContainsKey("x-enumNames").ShouldBeFalse();
        }

        [Fact]
        public void Dado_SchemaJaComXEnumNames_Quando_Apply_Entao_NaoDeveDuplicar()
        {
            // Dado
            var schema = new OpenApiSchema();
            schema.Extensions.Add("x-enumNames", new Microsoft.OpenApi.Any.OpenApiArray());
            var generator = new SchemaGenerator(new SchemaGeneratorOptions(), new JsonSerializerDataContractResolver(new System.Text.Json.JsonSerializerOptions()));
            var schemaRepo = new SchemaRepository();
            var context = new SchemaFilterContext(typeof(TestEnum), generator, schemaRepo);

            // Quando
            _sut.Apply(schema, context);

            // Entao
            schema.Extensions.Count.ShouldBe(1);
        }

        #endregion

        #region Instanciacao

        [Fact]
        public void Dado_SwaggerEnumSchemaFilter_Quando_CriarInstancia_Entao_DeveSerISchemaFilter()
        {
            _sut.ShouldNotBeNull();
            _sut.ShouldBeAssignableTo<Swashbuckle.AspNetCore.SwaggerGen.ISchemaFilter>();
        }

        #endregion
    }
}
