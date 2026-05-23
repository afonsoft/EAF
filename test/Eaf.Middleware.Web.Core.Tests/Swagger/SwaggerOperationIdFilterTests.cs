using Eaf.Middleware.Swagger;
using Shouldly;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Swagger
{
    public class SwaggerOperationIdFilterTests
    {
        [Fact]
        public void Dado_SwaggerOperationIdFilter_Quando_Criado_Entao_DeveImplementarIOperationFilter()
        {
            var filter = new SwaggerOperationIdFilter();
            filter.ShouldBeAssignableTo<IOperationFilter>();
        }
    }
}
