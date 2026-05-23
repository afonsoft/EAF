using Eaf.Middleware.Web.Swagger;
using Shouldly;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Swagger
{
    public class SwaggerOperationFilterTests
    {
        [Fact]
        public void Dado_SwaggerOperationFilter_Quando_Criado_Entao_DeveImplementarIOperationFilter()
        {
            var filter = new SwaggerOperationFilter();
            filter.ShouldBeAssignableTo<IOperationFilter>();
        }
    }
}
