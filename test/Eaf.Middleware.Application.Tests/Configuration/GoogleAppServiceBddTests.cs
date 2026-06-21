using Eaf.Middleware.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Configuration
{
    /// <summary>
    /// Testes BDD para GoogleAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class GoogleAppServiceBddTests
    {
        private readonly GoogleAppService _sut;

        public GoogleAppServiceBddTests()
        {
            _sut = new GoogleAppService();
        }

        #region Construtor

        [Fact]
        public void Dado_NenhumParametro_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            // Dado / Quando
            var sut = new GoogleAppService();

            // Então
            sut.ShouldNotBeNull();
        }

        #endregion
    }
}
