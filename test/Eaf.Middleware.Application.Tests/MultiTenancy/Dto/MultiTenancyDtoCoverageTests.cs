using Abp.Application.Services.Dto;
using Eaf.Middleware.Editions.Dto;
using Eaf.Middleware.MultiTenancy.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.MultiTenancy.Dto
{
    public class MultiTenancyDtoCoverageTests
    {
        [Fact]
        public void CreateTenantInput_ShouldSet()
        {
            var dto = new CreateTenantInput
            {
                AdminEmailAddress = "a@b.com",
                AdminPassword = "pw",
                IsActive = true,
                Name = "Tenant",
                SendActivationEmail = true,
                ShouldChangePasswordOnNextLogin = false,
                TenancyName = "tenant"
            };
            dto.AdminEmailAddress.ShouldBe("a@b.com");
            dto.AdminPassword.ShouldBe("pw");
            dto.IsActive.ShouldBeTrue();
            dto.Name.ShouldBe("Tenant");
            dto.SendActivationEmail.ShouldBeTrue();
            dto.ShouldChangePasswordOnNextLogin.ShouldBeFalse();
            dto.TenancyName.ShouldBe("tenant");
        }

        [Fact]
        public void GetTenantFeaturesEditOutput_ShouldSet()
        {
            var dto = new GetTenantFeaturesEditOutput
            {
                Features = new List<FlatFeatureDto>(),
                FeatureValues = new List<NameValueDto>()
            };
            dto.Features.ShouldNotBeNull();
            dto.FeatureValues.ShouldNotBeNull();
        }

        [Fact]
        public void GetTenantsInput_Normalize_DefaultsSorting()
        {
            var dto = new GetTenantsInput();
            dto.Filter.ShouldBe("");
            dto.Normalize();
            dto.Sorting.ShouldBe("TenancyName");
        }

        [Fact]
        public void GetTenantsInput_Normalize_PreservesSorting()
        {
            var dto = new GetTenantsInput { Sorting = "Name DESC" };
            dto.Normalize();
            dto.Sorting.ShouldBe("Name DESC");
        }

        [Fact]
        public void TenantAddressDto_ShouldSetAll()
        {
            var dto = new TenantAddressDto
            {
                ZipCode = "12345",
                Street = "Main",
                Neighborhood = "Downtown",
                City = "City",
                State = "SP",
                Complement = "A",
                Observation = "ob",
                Email = "e@a.com",
                Document = "doc",
                TenantId = 7,
                ExtensionData = "{}"
            };
            dto.ZipCode.ShouldBe("12345");
            dto.Street.ShouldBe("Main");
            dto.Neighborhood.ShouldBe("Downtown");
            dto.City.ShouldBe("City");
            dto.State.ShouldBe("SP");
            dto.Complement.ShouldBe("A");
            dto.Observation.ShouldBe("ob");
            dto.Email.ShouldBe("e@a.com");
            dto.Document.ShouldBe("doc");
            dto.TenantId.ShouldBe(7);
            dto.ExtensionData.ShouldBe("{}");
        }

        [Fact]
        public void TenantEditDto_ShouldSet()
        {
            var dto = new TenantEditDto { IsActive = true, Name = "n", TenancyName = "tn" };
            dto.IsActive.ShouldBeTrue();
            dto.Name.ShouldBe("n");
            dto.TenancyName.ShouldBe("tn");
        }

        [Fact]
        public void TenantListDto_LastModificationDate_FallsBackToCreationTime()
        {
            var dto = new TenantListDto
            {
                CreationTime = new DateTime(2020, 1, 1),
                LastModificationTime = null,
                IsActive = true,
                Name = "t",
                TenancyName = "tn"
            };
            dto.LastModificationDate.ShouldBe(new DateTime(2020, 1, 1));
            dto.IsActive.ShouldBeTrue();
            dto.Name.ShouldBe("t");
            dto.TenancyName.ShouldBe("tn");
        }

        [Fact]
        public void TenantListDto_LastModificationDate_UsesLastModificationTime()
        {
            var dto = new TenantListDto
            {
                CreationTime = new DateTime(2020, 1, 1),
                LastModificationTime = new DateTime(2021, 5, 1)
            };
            dto.LastModificationDate.ShouldBe(new DateTime(2021, 5, 1));
        }

        [Fact]
        public void UpdateTenantFeaturesInput_ShouldSet()
        {
            var dto = new UpdateTenantFeaturesInput
            {
                FeatureValues = new List<NameValueDto> { new NameValueDto("a", "1") },
                Id = 10
            };
            dto.FeatureValues.Count.ShouldBe(1);
            dto.Id.ShouldBe(10);
        }
    }
}
