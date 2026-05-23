using Eaf.Middleware.Web.Swagger;
using Shouldly;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Swagger
{
    public class SwaggerEnumParameterFilterTests
    {
        [Fact]
        public void Dado_SwaggerEnumParameterFilter_Quando_Criado_Entao_DeveImplementarIParameterFilter()
        {
            var filter = new SwaggerEnumParameterFilter();
            filter.ShouldBeAssignableTo<IParameterFilter>();
        }
    }
}
