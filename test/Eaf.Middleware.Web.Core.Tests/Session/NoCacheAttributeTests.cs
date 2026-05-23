using Eaf.Middleware.Web.Session;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Session
{
    public class NoCacheAttributeTests
    {
        [Fact]
        public void Dado_NoCacheAttribute_Quando_Verificado_Entao_DeveSerSealedClass()
        {
            typeof(NoCacheAttribute).IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void Dado_NoCacheAttribute_Quando_Verificado_Entao_DeveAceitarClasseEMetodo()
        {
            var attr = Attribute.GetCustomAttribute(typeof(NoCacheAttribute), typeof(AttributeUsageAttribute)) as AttributeUsageAttribute;

            attr.ShouldNotBeNull();
            attr!.ValidOn.ShouldBe(AttributeTargets.Class | AttributeTargets.Method);
        }

        [Fact]
        public void Dado_NoCacheAttribute_Quando_Criado_Entao_DeveSerInstanciaDeActionFilterAttribute()
        {
            var attribute = new NoCacheAttribute();
            attribute.ShouldBeAssignableTo<Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute>();
        }
    }
}
