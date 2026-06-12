using Eaf.Middleware.MultiTenancy;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.MultiTenancy
{
    /// <summary>
    /// Testes BDD para Tenant e TenantAddress seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class TenantAndAddressBddTests
    {
        #region Tenant

        [Fact]
        public void Dado_Tenant_Quando_CriarComParametros_Entao_DeveDefinirPropriedades()
        {
            var tenant = new Tenant("acme", "Acme Corp");
            tenant.TenancyName.ShouldBe("acme");
            tenant.Name.ShouldBe("Acme Corp");
        }

        #endregion

        #region TenantAddress IsValid

        [Fact]
        public void Dado_EnderecoCompleto_Quando_IsValid_Entao_DeveRetornarTrue()
        {
            var address = new TenantAddress
            {
                ZipCode = "01310-100",
                Street = "Av. Paulista",
                Neighborhood = "Bela Vista",
                City = "São Paulo",
                State = "SP"
            };

            address.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_EnderecoSemZipCode_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var address = new TenantAddress
            {
                Street = "Av. Paulista",
                Neighborhood = "Bela Vista",
                City = "São Paulo",
                State = "SP"
            };

            address.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_EnderecoSemStreet_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var address = new TenantAddress
            {
                ZipCode = "01310-100",
                Neighborhood = "Bela Vista",
                City = "São Paulo",
                State = "SP"
            };

            address.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_EnderecoSemCity_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var address = new TenantAddress
            {
                ZipCode = "01310-100",
                Street = "Av. Paulista",
                Neighborhood = "Bela Vista",
                State = "SP"
            };

            address.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_EnderecoSemState_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var address = new TenantAddress
            {
                ZipCode = "01310-100",
                Street = "Av. Paulista",
                Neighborhood = "Bela Vista",
                City = "São Paulo"
            };

            address.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_EnderecoSemNeighborhood_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var address = new TenantAddress
            {
                ZipCode = "01310-100",
                Street = "Av. Paulista",
                City = "São Paulo",
                State = "SP"
            };

            address.IsValid().ShouldBeFalse();
        }

        #endregion

        #region TenantAddress GetFullAddress

        [Fact]
        public void Dado_EnderecoCompleto_Quando_GetFullAddress_Entao_DeveRetornarFormatado()
        {
            var address = new TenantAddress
            {
                Street = "Av. Paulista",
                Neighborhood = "Bela Vista",
                City = "São Paulo",
                State = "SP",
                ZipCode = "01310-100"
            };

            var fullAddress = address.GetFullAddress();
            fullAddress.ShouldContain("Av. Paulista");
            fullAddress.ShouldContain("Bela Vista");
            fullAddress.ShouldContain("São Paulo");
            fullAddress.ShouldContain("SP");
            fullAddress.ShouldContain("01310-100");
        }

        [Fact]
        public void Dado_EnderecoApenasRua_Quando_GetFullAddress_Entao_DeveRetornarApenasRua()
        {
            var address = new TenantAddress
            {
                Street = "Av. Paulista"
            };

            var fullAddress = address.GetFullAddress();
            fullAddress.ShouldBe("Av. Paulista");
        }

        [Fact]
        public void Dado_EnderecoVazio_Quando_GetFullAddress_Entao_DeveRetornarVazio()
        {
            var address = new TenantAddress();
            var fullAddress = address.GetFullAddress();
            fullAddress.ShouldBeEmpty();
        }

        #endregion

        #region TenantAddress Properties

        [Fact]
        public void Dado_TenantAddress_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var address = new TenantAddress
            {
                TenantId = 1,
                ZipCode = "01310-100",
                Street = "Av. Paulista, 1000",
                Neighborhood = "Bela Vista",
                City = "São Paulo",
                State = "SP",
                Complement = "10º andar",
                Observation = "Próximo ao metrô",
                Email = "contato@acme.com",
                Document = "12.345.678/0001-90",
                IsActive = true,
                ExtensionData = "{\"country\":\"BR\"}"
            };

            address.TenantId.ShouldBe(1);
            address.Complement.ShouldBe("10º andar");
            address.Observation.ShouldBe("Próximo ao metrô");
            address.Email.ShouldBe("contato@acme.com");
            address.Document.ShouldBe("12.345.678/0001-90");
            address.IsActive.ShouldBeTrue();
            address.ExtensionData.ShouldContain("BR");
        }

        [Fact]
        public void Dado_TenantAddress_Quando_CriarNovo_Entao_IsActiveDeveSerTrue()
        {
            var address = new TenantAddress();
            address.IsActive.ShouldBeTrue();
        }

        #endregion
    }
}
