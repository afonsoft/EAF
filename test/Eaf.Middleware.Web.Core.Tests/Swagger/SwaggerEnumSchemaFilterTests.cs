using Eaf.Middleware.Web.Swagger;
using Shouldly;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Swagger
{
    public class SwaggerEnumSchemaFilterTests
    {
        [Fact]
        public void Dado_SwaggerEnumSchemaFilter_Quando_Criado_Entao_DeveImplementarISchemaFilter()
        {
            var filter = new SwaggerEnumSchemaFilter();
            filter.ShouldBeAssignableTo<ISchemaFilter>();
        }
    }
}
