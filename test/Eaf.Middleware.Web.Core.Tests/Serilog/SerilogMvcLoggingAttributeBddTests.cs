using Eaf.Middleware.Web.Serilog;
using Microsoft.AspNetCore.Mvc.Filters;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Serilog
{
    public class SerilogMvcLoggingAttributeBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new SerilogMvcLoggingAttribute();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_VerificarHeranca_Entao_DeveSerActionFilterAttribute()
        {
            var sut = new SerilogMvcLoggingAttribute();
            sut.ShouldBeAssignableTo<ActionFilterAttribute>();
        }
    }
}
