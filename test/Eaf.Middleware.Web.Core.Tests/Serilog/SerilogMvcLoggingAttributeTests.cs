using Eaf.Middleware.Web.Serilog;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Serilog
{
    public class SerilogMvcLoggingAttributeTests
    {
        [Fact]
        public void Dado_SerilogMvcLoggingAttribute_Quando_Criado_Entao_DeveSerInstanciaDeActionFilterAttribute()
        {
            var attribute = new SerilogMvcLoggingAttribute();
            attribute.ShouldBeAssignableTo<Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute>();
        }

        [Fact]
        public void Dado_SerilogMvcLoggingAttribute_Quando_Verificado_Entao_DeveAceitarClasseEMetodo()
        {
            var attr = Attribute.GetCustomAttribute(typeof(SerilogMvcLoggingAttribute), typeof(AttributeUsageAttribute)) as AttributeUsageAttribute;

            attr.ShouldNotBeNull();
            attr!.ValidOn.ShouldBe(AttributeTargets.Class | AttributeTargets.Method);
        }
    }
}
