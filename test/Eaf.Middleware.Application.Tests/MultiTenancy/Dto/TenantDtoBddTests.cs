using Abp.Application.Services.Dto;
using Eaf.Middleware.Editions.Dto;
using Eaf.Middleware.MultiTenancy.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.MultiTenancy.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de Tenant seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class TenantDtoBddTests
    {
        #region GetTenantsInput

        [Fact]
        public void Dado_GetTenantsInput_Quando_Criar_Entao_FilterDeveSerVazio()
        {
            var input = new GetTenantsInput();
            input.Filter.ShouldBe("");
        }

        [Fact]
        public void Dado_GetTenantsInput_Quando_NormalizeSemSorting_Entao_DeveDefinirComoTenancyName()
        {
            var input = new GetTenantsInput();
            input.Normalize();
            input.Sorting.ShouldBe("TenancyName");
        }

        [Fact]
        public void Dado_GetTenantsInput_Quando_NormalizeComSorting_Entao_NaoDeveAlterar()
        {
            var input = new GetTenantsInput { Sorting = "Name" };
            input.Normalize();
            input.Sorting.ShouldBe("Name");
        }

        #endregion

        #region TenantListDto

        [Fact]
        public void Dado_TenantListDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new TenantListDto
            {
                Name = "Acme Corp",
                TenancyName = "acme",
                IsActive = true
            };

            dto.Name.ShouldBe("Acme Corp");
            dto.TenancyName.ShouldBe("acme");
            dto.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void Dado_TenantListDto_Quando_LastModificationTimeNull_Entao_DeveRetornarCreationTime()
        {
            var creationTime = new DateTime(2026, 1, 1);
            var dto = new TenantListDto { CreationTime = creationTime };
            dto.LastModificationDate.ShouldBe(creationTime);
        }

        [Fact]
        public void Dado_TenantListDto_Quando_LastModificationTimePreenchido_Entao_DeveRetornarLastModificationTime()
        {
            var modTime = new DateTime(2026, 6, 1);
            var dto = new TenantListDto
            {
                CreationTime = new DateTime(2026, 1, 1),
                LastModificationTime = modTime
            };
            dto.LastModificationDate.ShouldBe(modTime);
        }

        #endregion

        #region TenantEditDto

        [Fact]
        public void Dado_TenantEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new TenantEditDto
            {
                Id = 1,
                Name = "Acme",
                TenancyName = "acme",
                IsActive = true
            };

            dto.Id.ShouldBe(1);
            dto.Name.ShouldBe("Acme");
            dto.TenancyName.ShouldBe("acme");
            dto.IsActive.ShouldBeTrue();
        }

        #endregion

        #region CreateTenantInput

        [Fact]
        public void Dado_CreateTenantInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new CreateTenantInput
            {
                TenancyName = "newcorp",
                Name = "New Corp",
                AdminEmailAddress = "admin@newcorp.com",
                AdminPassword = "P@ss1234",
                IsActive = true,
                SendActivationEmail = false,
                ShouldChangePasswordOnNextLogin = true
            };

            input.TenancyName.ShouldBe("newcorp");
            input.Name.ShouldBe("New Corp");
            input.AdminEmailAddress.ShouldBe("admin@newcorp.com");
            input.AdminPassword.ShouldBe("P@ss1234");
            input.IsActive.ShouldBeTrue();
            input.SendActivationEmail.ShouldBeFalse();
            input.ShouldChangePasswordOnNextLogin.ShouldBeTrue();
        }

        #endregion

        #region TenantAddressDto

        [Fact]
        public void Dado_TenantAddressDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new TenantAddressDto
            {
                ZipCode = "01001-000",
                Street = "Praça da Sé",
                Neighborhood = "Sé",
                City = "São Paulo",
                State = "SP",
                Complement = "Apto 1",
                Observation = "Obs",
                Email = "contato@acme.com",
                Document = "12345678901",
                TenantId = 1
            };

            dto.ZipCode.ShouldBe("01001-000");
            dto.Street.ShouldBe("Praça da Sé");
            dto.Neighborhood.ShouldBe("Sé");
            dto.City.ShouldBe("São Paulo");
            dto.State.ShouldBe("SP");
            dto.Complement.ShouldBe("Apto 1");
            dto.Observation.ShouldBe("Obs");
            dto.Email.ShouldBe("contato@acme.com");
            dto.Document.ShouldBe("12345678901");
            dto.TenantId.ShouldBe(1);
        }

        #endregion

        #region UpdateTenantFeaturesInput

        [Fact]
        public void Dado_UpdateTenantFeaturesInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new UpdateTenantFeaturesInput
            {
                Id = 1,
                FeatureValues = new List<NameValueDto>
                {
                    new NameValueDto { Name = "Feature1", Value = "true" }
                }
            };

            input.Id.ShouldBe(1);
            input.FeatureValues.Count.ShouldBe(1);
        }

        #endregion

        #region GetTenantFeaturesEditOutput

        [Fact]
        public void Dado_GetTenantFeaturesEditOutput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var output = new GetTenantFeaturesEditOutput
            {
                FeatureValues = new List<NameValueDto>(),
                Features = new List<FlatFeatureDto>()
            };

            output.FeatureValues.ShouldNotBeNull();
            output.Features.ShouldNotBeNull();
        }

        #endregion
    }
}
