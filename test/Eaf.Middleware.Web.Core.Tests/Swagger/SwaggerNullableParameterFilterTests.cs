using Eaf.Middleware.Web.Swagger;
using Shouldly;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Swagger
{
    public class SwaggerNullableParameterFilterTests
    {
        [Fact]
        public void Dado_SwaggerNullableParameterFilter_Quando_Criado_Entao_DeveImplementarIParameterFilter()
        {
            var filter = new SwaggerNullableParameterFilter();
            filter.ShouldBeAssignableTo<IParameterFilter>();
        }
    }
}
