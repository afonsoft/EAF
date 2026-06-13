using Eaf.Middleware.MultiTenancy.Dto;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.MultiTenancy.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de MultiTenancy seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class MultiTenancyDtoBddTests
    {
        #region CreateTenantInput

        [Fact]
        public void Dado_CreateTenantInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new CreateTenantInput
            {
                TenancyName = "acme",
                Name = "Acme Corp",
                AdminEmailAddress = "admin@acme.com",
                AdminPassword = "Admin@123",
                IsActive = true,
                SendActivationEmail = false,
                ShouldChangePasswordOnNextLogin = true
            };

            input.TenancyName.ShouldBe("acme");
            input.Name.ShouldBe("Acme Corp");
            input.AdminEmailAddress.ShouldBe("admin@acme.com");
            input.AdminPassword.ShouldBe("Admin@123");
            input.IsActive.ShouldBeTrue();
            input.SendActivationEmail.ShouldBeFalse();
            input.ShouldChangePasswordOnNextLogin.ShouldBeTrue();
        }

        #endregion

        #region GetTenantsInput

        [Fact]
        public void Dado_GetTenantsInput_SemSorting_Quando_Normalize_Entao_DeveDefinirPadrao()
        {
            var input = new GetTenantsInput();
            input.Normalize();
            input.Sorting.ShouldBe("TenancyName");
        }

        [Fact]
        public void Dado_GetTenantsInput_ComSorting_Quando_Normalize_Entao_DeveManterValor()
        {
            var input = new GetTenantsInput { Sorting = "Name ASC" };
            input.Normalize();
            input.Sorting.ShouldBe("Name ASC");
        }

        [Fact]
        public void Dado_GetTenantsInput_Quando_VerificarFilterPadrao_Entao_DeveSerVazio()
        {
            var input = new GetTenantsInput();
            input.Filter.ShouldBe("");
        }

        #endregion

        #region TenantListDto

        [Fact]
        public void Dado_TenantListDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new TenantListDto
            {
                Id = 1,
                TenancyName = "acme",
                Name = "Acme Corp",
                IsActive = true
            };

            dto.Id.ShouldBe(1);
            dto.TenancyName.ShouldBe("acme");
            dto.Name.ShouldBe("Acme Corp");
            dto.IsActive.ShouldBeTrue();
        }

        #endregion

        #region TenantEditDto

        [Fact]
        public void Dado_TenantEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new TenantEditDto
            {
                TenancyName = "acme",
                Name = "Acme Corp Updated",
                IsActive = false
            };

            dto.TenancyName.ShouldBe("acme");
            dto.Name.ShouldBe("Acme Corp Updated");
            dto.IsActive.ShouldBeFalse();
        }

        #endregion

        #region TenantAddressDto

        [Fact]
        public void Dado_TenantAddressDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new TenantAddressDto
            {
                ZipCode = "01310-100",
                Street = "Av. Paulista",
                Neighborhood = "Bela Vista",
                City = "São Paulo",
                State = "SP",
                Complement = "Sala 1001",
                Observation = "Entrada principal",
                Email = "admin@acme.com",
                Document = "12.345.678/0001-90",
                TenantId = 1
            };

            dto.ZipCode.ShouldBe("01310-100");
            dto.Street.ShouldBe("Av. Paulista");
            dto.Neighborhood.ShouldBe("Bela Vista");
            dto.City.ShouldBe("São Paulo");
            dto.State.ShouldBe("SP");
            dto.Complement.ShouldBe("Sala 1001");
            dto.Observation.ShouldBe("Entrada principal");
            dto.Email.ShouldBe("admin@acme.com");
            dto.Document.ShouldBe("12.345.678/0001-90");
            dto.TenantId.ShouldBe(1);
        }

        #endregion

        #region GetTenantFeaturesEditOutput

        [Fact]
        public void Dado_GetTenantFeaturesEditOutput_Quando_DefinirListas_Entao_DeveArmazenar()
        {
            var output = new GetTenantFeaturesEditOutput
            {
                Features = new List<Eaf.Middleware.Editions.Dto.FlatFeatureDto>(),
                FeatureValues = new List<Abp.Application.Services.Dto.NameValueDto>()
            };

            output.Features.ShouldNotBeNull();
            output.FeatureValues.ShouldNotBeNull();
        }

        #endregion
    }
}
