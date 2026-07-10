using Eaf.Middleware.Web.Serilog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using Serilog;
using Serilog.Context;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Serilog
{
    public class SerilogMvcLoggingAttributeBddTests
    {
        [Fact]
        public void Dado_DiagnosticContextDisponivel_Quando_ExecutarOnActionExecuting_Entao_DeveDefinirPropriedades()
        {
            var diagnosticContext = Substitute.For<IDiagnosticContext>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IDiagnosticContext)).Returns(diagnosticContext);

            var attribute = new SerilogMvcLoggingAttribute();
            var context = CriarActionExecutingContext(serviceProvider);

            attribute.OnActionExecuting(context);

            diagnosticContext.Received(1).Set("ActionName", context.ActionDescriptor.DisplayName);
            diagnosticContext.Received(1).Set("ActionId", Arg.Any<string>());
            diagnosticContext.Received(1).Set("RouteData", context.ActionDescriptor.RouteValues);
            diagnosticContext.Received(1).Set("ValidationState", context.ModelState.IsValid);
        }

        [Fact]
        public void Dado_SemDiagnosticContext_Quando_ExecutarOnActionExecuting_Entao_NaoDeveLancarExcecao()
        {
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IDiagnosticContext)).Returns(null);

            var attribute = new SerilogMvcLoggingAttribute();
            var context = CriarActionExecutingContext(serviceProvider);

            Should.NotThrow(() => attribute.OnActionExecuting(context));
        }

        private static ActionExecutingContext CriarActionExecutingContext(IServiceProvider serviceProvider)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = serviceProvider;

            var actionDescriptor = new ActionDescriptor
            {
                DisplayName = "TestAction",
                RouteValues = new Dictionary<string, string?> { { "controller", "Test" }, { "action", "Index" } }
            };

            var actionContext = new ActionContext(httpContext, new RouteData(), actionDescriptor);
            return new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), new object());
        }
    }
}
