using Eaf.Middleware.Editions.Dto;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Dto
{
    public class FeatureDtoBddTests
    {
        [Fact]
        public void Dado_FlatFeatureDto_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var dto = new FlatFeatureDto
            {
                Name = "ChatFeature",
                DisplayName = "Chat",
                Description = "Permite usar o chat",
                DefaultValue = "true",
                ParentName = null,
                InputType = new FeatureInputTypeDto { Name = "CHECKBOX" }
            };

            dto.Name.ShouldBe("ChatFeature");
            dto.DisplayName.ShouldBe("Chat");
            dto.Description.ShouldBe("Permite usar o chat");
            dto.DefaultValue.ShouldBe("true");
            dto.ParentName.ShouldBeNull();
            dto.InputType.ShouldNotBeNull();
            dto.InputType.Name.ShouldBe("CHECKBOX");
        }

        [Fact]
        public void Dado_FlatFeatureSelectDto_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var dto = new FlatFeatureSelectDto
            {
                Name = "MaxUsers",
                DisplayName = "Máximo de Usuários",
                Description = "Limite de usuários",
                DefaultValue = "10",
                ParentName = "TenantFeatures",
                TextHtmlColor = "green"
            };

            dto.Name.ShouldBe("MaxUsers");
            dto.DisplayName.ShouldBe("Máximo de Usuários");
            dto.Description.ShouldBe("Limite de usuários");
            dto.DefaultValue.ShouldBe("10");
            dto.ParentName.ShouldBe("TenantFeatures");
            dto.TextHtmlColor.ShouldBe("green");
        }

        [Fact]
        public void Dado_FeatureInputTypeDto_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var dto = new FeatureInputTypeDto
            {
                Name = "SINGLE_LINE_STRING",
                Attributes = new Dictionary<string, object> { { "MinValue", 1 }, { "MaxValue", 100 } }
            };

            dto.Name.ShouldBe("SINGLE_LINE_STRING");
            dto.Attributes.ShouldNotBeNull();
            dto.Attributes.Count.ShouldBe(2);
            dto.Attributes["MinValue"].ShouldBe(1);
        }
    }
}
