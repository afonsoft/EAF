using Eaf.Middleware.Web.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Session
{
    /// <summary>
    /// Testes BDD para NoCacheAttribute seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class NoCacheAttributeBddTests
    {
        private ResultExecutingContext CreateContext()
        {
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            return new ResultExecutingContext(actionContext, new List<IFilterMetadata>(), new OkResult(), new object());
        }

        #region OnResultExecuting

        [Fact]
        public void Dado_NoCacheAttribute_Quando_OnResultExecuting_Entao_DeveDefinirCacheControlNoCache()
        {
            // Dado
            var attribute = new NoCacheAttribute();
            var context = CreateContext();

            // Quando
            attribute.OnResultExecuting(context);

            // Entao
            context.HttpContext.Response.Headers["Cache-Control"].ToString().ShouldBe("no-cache, no-store, must-revalidate");
        }

        [Fact]
        public void Dado_NoCacheAttribute_Quando_OnResultExecuting_Entao_DeveDefinirExpiresNegativo()
        {
            // Dado
            var attribute = new NoCacheAttribute();
            var context = CreateContext();

            // Quando
            attribute.OnResultExecuting(context);

            // Entao
            context.HttpContext.Response.Headers["Expires"].ToString().ShouldBe("-1");
        }

        [Fact]
        public void Dado_NoCacheAttribute_Quando_OnResultExecuting_Entao_DeveDefinirPragmaNoCache()
        {
            // Dado
            var attribute = new NoCacheAttribute();
            var context = CreateContext();

            // Quando
            attribute.OnResultExecuting(context);

            // Entao
            context.HttpContext.Response.Headers["Pragma"].ToString().ShouldBe("no-cache");
        }

        #endregion

        #region Atributo

        [Fact]
        public void Dado_NoCacheAttribute_Quando_VerificarUsage_Entao_DevePermitirClasseEMetodo()
        {
            var attr = (AttributeUsageAttribute)Attribute.GetCustomAttribute(typeof(NoCacheAttribute), typeof(AttributeUsageAttribute));
            attr.ShouldNotBeNull();
            attr.ValidOn.HasFlag(AttributeTargets.Class).ShouldBeTrue();
            attr.ValidOn.HasFlag(AttributeTargets.Method).ShouldBeTrue();
        }

        [Fact]
        public void Dado_NoCacheAttribute_Quando_Verificar_Entao_DeveSerSealed()
        {
            typeof(NoCacheAttribute).IsSealed.ShouldBeTrue();
        }

        #endregion
    }
}
