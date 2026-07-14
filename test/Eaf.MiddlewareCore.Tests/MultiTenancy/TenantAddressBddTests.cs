using Eaf.Middleware.MultiTenancy;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.MultiTenancy
{
    public class TenantAddressBddTests
    {
        #region IsValid

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

        [Theory]
        [InlineData(null, "Rua A", "Centro", "SP", "SP")]
        [InlineData("", "Rua A", "Centro", "SP", "SP")]
        [InlineData("01310", null, "Centro", "SP", "SP")]
        [InlineData("01310", "", "Centro", "SP", "SP")]
        [InlineData("01310", "Rua A", null, "SP", "SP")]
        [InlineData("01310", "Rua A", "", "SP", "SP")]
        [InlineData("01310", "Rua A", "Centro", null, "SP")]
        [InlineData("01310", "Rua A", "Centro", "", "SP")]
        [InlineData("01310", "Rua A", "Centro", "SP", null)]
        [InlineData("01310", "Rua A", "Centro", "SP", "")]
        public void Dado_EnderecoParcial_Quando_IsValid_Entao_DeveRetornarFalse(
            string zipCode, string street, string neighborhood, string city, string state)
        {
            var address = new TenantAddress
            {
                ZipCode = zipCode,
                Street = street,
                Neighborhood = neighborhood,
                City = city,
                State = state
            };

            address.IsValid().ShouldBeFalse();
        }

        #endregion

        #region GetFullAddress

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

            var result = address.GetFullAddress();

            result.ShouldBe("Av. Paulista, Bela Vista, São Paulo - SP, 01310-100");
        }

        [Fact]
        public void Dado_ApenasRua_Quando_GetFullAddress_Entao_DeveRetornarSoRua()
        {
            var address = new TenantAddress { Street = "Av. Paulista" };

            address.GetFullAddress().ShouldBe("Av. Paulista");
        }

        [Fact]
        public void Dado_ApenasBairro_Quando_GetFullAddress_Entao_DeveRetornarSoBairro()
        {
            var address = new TenantAddress { Neighborhood = "Bela Vista" };

            address.GetFullAddress().ShouldBe("Bela Vista");
        }

        [Fact]
        public void Dado_ApenasCidade_Quando_GetFullAddress_Entao_DeveRetornarSoCidade()
        {
            var address = new TenantAddress { City = "São Paulo" };

            address.GetFullAddress().ShouldBe("São Paulo");
        }

        [Fact]
        public void Dado_ApenasEstado_Quando_GetFullAddress_Entao_DeveRetornarSoEstado()
        {
            var address = new TenantAddress { State = "SP" };

            address.GetFullAddress().ShouldBe("SP");
        }

        [Fact]
        public void Dado_ApenasCep_Quando_GetFullAddress_Entao_DeveRetornarSoCep()
        {
            var address = new TenantAddress { ZipCode = "01310-100" };

            address.GetFullAddress().ShouldBe("01310-100");
        }

        [Fact]
        public void Dado_EnderecoVazio_Quando_GetFullAddress_Entao_DeveRetornarStringVazia()
        {
            var address = new TenantAddress();

            address.GetFullAddress().ShouldBe(string.Empty);
        }

        [Fact]
        public void Dado_RuaECidade_Quando_GetFullAddress_Entao_DeveRetornarFormatado()
        {
            var address = new TenantAddress
            {
                Street = "Av. Paulista",
                City = "São Paulo"
            };

            address.GetFullAddress().ShouldBe("Av. Paulista, São Paulo");
        }

        [Fact]
        public void Dado_CidadeEEstado_Quando_GetFullAddress_Entao_DeveRetornarFormatado()
        {
            var address = new TenantAddress
            {
                City = "São Paulo",
                State = "SP"
            };

            address.GetFullAddress().ShouldBe("São Paulo - SP");
        }

        #endregion

        #region Properties

        [Fact]
        public void Dado_TenantAddress_Quando_CriarNovo_Entao_IsActiveDeveSerTrue()
        {
            var address = new TenantAddress();

            address.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void Dado_TenantAddress_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var address = new TenantAddress
            {
                ZipCode = "01310-100",
                Street = "Av. Paulista, 1000",
                Neighborhood = "Bela Vista",
                City = "São Paulo",
                State = "SP",
                Complement = "Sala 101",
                Observation = "Prédio Comercial",
                Email = "contato@empresa.com",
                Document = "12.345.678/0001-90",
                TenantId = 1,
                ExtensionData = "{\"key\":\"value\"}",
                IsActive = false
            };

            address.ZipCode.ShouldBe("01310-100");
            address.Street.ShouldBe("Av. Paulista, 1000");
            address.Neighborhood.ShouldBe("Bela Vista");
            address.City.ShouldBe("São Paulo");
            address.State.ShouldBe("SP");
            address.Complement.ShouldBe("Sala 101");
            address.Observation.ShouldBe("Prédio Comercial");
            address.Email.ShouldBe("contato@empresa.com");
            address.Document.ShouldBe("12.345.678/0001-90");
            address.TenantId.ShouldBe(1);
            address.ExtensionData.ShouldBe("{\"key\":\"value\"}");
            address.IsActive.ShouldBeFalse();
        }

        #endregion

        #region Tenant

        [Fact]
        public void Dado_Tenant_Quando_CriarComNome_Entao_DeveDefinirPropriedades()
        {
            var tenant = new Tenant("acme", "Acme Corp");

            tenant.TenancyName.ShouldBe("acme");
            tenant.Name.ShouldBe("Acme Corp");
        }

        [Fact]
        public void Dado_Tenant_Quando_CriarNovo_Entao_AddressesDeveSerNull()
        {
            var tenant = new Tenant("test", "Test");

            tenant.Addresses.ShouldBeNull();
        }

        [Fact]
        public void Dado_TenantAddress_Quando_DefinirTenant_Entao_DevePersistir()
        {
            var tenant = new Tenant("acme", "Acme Corp") { Id = 1 };
            var address = new TenantAddress();

            address.Tenant = tenant;

            address.Tenant.ShouldNotBeNull();
            address.Tenant.Id.ShouldBe(1);
        }

        #endregion
    }
}
