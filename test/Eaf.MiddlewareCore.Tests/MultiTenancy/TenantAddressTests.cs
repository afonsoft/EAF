using Eaf.Middleware.MultiTenancy;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.MultiTenancy
{
    /// <summary>
    /// Testes BDD para TenantAddress seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class TenantAddressTests
    {
        #region Construtor e Propriedades

        [Fact]
        public void Dado_NovaInstancia_Quando_CriarTenantAddress_Entao_IsActiveDeveSerTrue()
        {
            // Dado & Quando
            var address = new TenantAddress();

            // Então
            address.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void Dado_TenantAddress_Quando_DefinirTodasPropriedades_Entao_DeveArmazenarCorretamente()
        {
            // Dado
            var address = new TenantAddress();

            // Quando
            address.ZipCode = "01001-000";
            address.Street = "Praça da Sé";
            address.Neighborhood = "Sé";
            address.City = "São Paulo";
            address.State = "SP";
            address.Complement = "Lado ímpar";
            address.Observation = "Centro histórico";
            address.Email = "contato@exemplo.com.br";
            address.Document = "12.345.678/0001-90";
            address.TenantId = 42;

            // Então
            address.ZipCode.ShouldBe("01001-000");
            address.Street.ShouldBe("Praça da Sé");
            address.Neighborhood.ShouldBe("Sé");
            address.City.ShouldBe("São Paulo");
            address.State.ShouldBe("SP");
            address.Complement.ShouldBe("Lado ímpar");
            address.Observation.ShouldBe("Centro histórico");
            address.Email.ShouldBe("contato@exemplo.com.br");
            address.Document.ShouldBe("12.345.678/0001-90");
            address.TenantId.ShouldBe(42);
        }

        [Fact]
        public void Dado_TenantAddress_Quando_DefinirExtensionData_Entao_DeveArmazenarJson()
        {
            // Dado
            var address = new TenantAddress();

            // Quando
            address.ExtensionData = "{\"tipo\":\"comercial\"}";

            // Então
            address.ExtensionData.ShouldBe("{\"tipo\":\"comercial\"}");
        }

        #endregion

        #region IsValid

        [Fact]
        public void Dado_EnderecoCompleto_Quando_Validar_Entao_DeveRetornarTrue()
        {
            // Dado
            var address = new TenantAddress
            {
                ZipCode = "01001-000",
                Street = "Praça da Sé",
                Neighborhood = "Sé",
                City = "São Paulo",
                State = "SP"
            };

            // Quando
            var result = address.IsValid();

            // Então
            result.ShouldBeTrue();
        }

        [Theory]
        [InlineData(null, "Rua A", "Bairro", "Cidade", "SP")]
        [InlineData("", "Rua A", "Bairro", "Cidade", "SP")]
        [InlineData("01001", null, "Bairro", "Cidade", "SP")]
        [InlineData("01001", "", "Bairro", "Cidade", "SP")]
        [InlineData("01001", "Rua A", null, "Cidade", "SP")]
        [InlineData("01001", "Rua A", "", "Cidade", "SP")]
        [InlineData("01001", "Rua A", "Bairro", null, "SP")]
        [InlineData("01001", "Rua A", "Bairro", "", "SP")]
        [InlineData("01001", "Rua A", "Bairro", "Cidade", null)]
        [InlineData("01001", "Rua A", "Bairro", "Cidade", "")]
        public void Dado_CampoObrigatorioAusente_Quando_Validar_Entao_DeveRetornarFalse(
            string zipCode, string street, string neighborhood, string city, string state)
        {
            // Dado
            var address = new TenantAddress
            {
                ZipCode = zipCode,
                Street = street,
                Neighborhood = neighborhood,
                City = city,
                State = state
            };

            // Quando
            var result = address.IsValid();

            // Então
            result.ShouldBeFalse();
        }

        #endregion

        #region GetFullAddress

        [Fact]
        public void Dado_EnderecoCompleto_Quando_ObterEnderecoCompleto_Entao_DeveFormatarCorretamente()
        {
            // Dado
            var address = new TenantAddress
            {
                Street = "Praça da Sé",
                Neighborhood = "Sé",
                City = "São Paulo",
                State = "SP",
                ZipCode = "01001-000"
            };

            // Quando
            var full = address.GetFullAddress();

            // Então
            full.ShouldBe("Praça da Sé, Sé, São Paulo - SP, 01001-000");
        }

        [Fact]
        public void Dado_ApenasCidade_Quando_ObterEnderecoCompleto_Entao_DeveRetornarApenasCidade()
        {
            // Dado
            var address = new TenantAddress { City = "Curitiba" };

            // Quando
            var full = address.GetFullAddress();

            // Então
            full.ShouldBe("Curitiba");
        }

        [Fact]
        public void Dado_EnderecoVazio_Quando_ObterEnderecoCompleto_Entao_DeveRetornarVazio()
        {
            // Dado
            var address = new TenantAddress();

            // Quando
            var full = address.GetFullAddress();

            // Então
            full.ShouldBeEmpty();
        }

        [Fact]
        public void Dado_ApenasRuaECep_Quando_ObterEnderecoCompleto_Entao_DeveFormatarSemSeparadorDeEstado()
        {
            // Dado
            var address = new TenantAddress
            {
                Street = "Av. Paulista",
                ZipCode = "01310-100"
            };

            // Quando
            var full = address.GetFullAddress();

            // Então
            full.ShouldBe("Av. Paulista, 01310-100");
        }

        #endregion
    }
}
