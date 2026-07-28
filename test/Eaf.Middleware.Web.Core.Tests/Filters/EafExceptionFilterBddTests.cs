using Abp.UI;
using Eaf.Middleware.Contracts;
using Eaf.Middleware.Web.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Tests.Filters
{
    public class EafExceptionFilterBddTests
    {
        [Fact]
        public void Dado_UserFriendlyException_Quando_OnException_Entao_DeveRetornar400()
        {
            // Dado
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor(),
                new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary());

            var context = new ExceptionContext(actionContext, new System.Collections.Generic.List<IFilterMetadata>())
            {
                Exception = new UserFriendlyException("Validation failed")
            };

            var filter = new EafExceptionFilter();

            // Quando
            filter.OnException(context);

            // Então
            context.ExceptionHandled.ShouldBeTrue();
            var result = context.Result.ShouldBeOfType<ObjectResult>();
            result.StatusCode.ShouldBe(400);
            var contract = result.Value.ShouldBeOfType<PublicErrorContract>();
            contract.Code.ShouldBe(EafErrorCodes.ValidationFailed);
            contract.Message.ShouldBe("Validation failed");
        }
    }
}
