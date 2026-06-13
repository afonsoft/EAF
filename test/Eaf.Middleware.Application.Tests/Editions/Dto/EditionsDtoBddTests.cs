using Eaf.Middleware.Editions.Dto;
using Shouldly;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Editions.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de Editions seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class EditionsDtoBddTests
    {
        #region FlatFeatureDto

        [Fact]
        public void Dado_FlatFeatureDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new FlatFeatureDto
            {
                Name = "App.ChatFeature",
                DisplayName = "Chat",
                Description = "Habilita chat entre usuários",
                DefaultValue = "true",
                ParentName = null,
                InputType = new FeatureInputTypeDto { Name = "CHECKBOX" }
            };

            dto.Name.ShouldBe("App.ChatFeature");
            dto.DisplayName.ShouldBe("Chat");
            dto.Description.ShouldBe("Habilita chat entre usuários");
            dto.DefaultValue.ShouldBe("true");
            dto.ParentName.ShouldBeNull();
            dto.InputType.ShouldNotBeNull();
            dto.InputType.Name.ShouldBe("CHECKBOX");
        }

        #endregion

        #region FlatFeatureSelectDto

        [Fact]
        public void Dado_FlatFeatureSelectDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new FlatFeatureSelectDto
            {
                Name = "App.MaxUsers",
                DisplayName = "Máximo de Usuários",
                DefaultValue = "100",
                TextHtmlColor = "#FF0000"
            };

            dto.Name.ShouldBe("App.MaxUsers");
            dto.DisplayName.ShouldBe("Máximo de Usuários");
            dto.DefaultValue.ShouldBe("100");
            dto.TextHtmlColor.ShouldBe("#FF0000");
        }

        #endregion

        #region FeatureInputTypeDto

        [Fact]
        public void Dado_FeatureInputTypeDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new FeatureInputTypeDto
            {
                Name = "COMBOBOX",
                ItemSource = new LocalizableComboboxItemSourceDto
                {
                    Items = new Collection<LocalizableComboboxItemDto>
                    {
                        new LocalizableComboboxItemDto { Value = "1", DisplayText = "Opção 1" },
                        new LocalizableComboboxItemDto { Value = "2", DisplayText = "Opção 2" }
                    }
                },
                Attributes = new Dictionary<string, object> { { "min", 0 }, { "max", 100 } }
            };

            dto.Name.ShouldBe("COMBOBOX");
            dto.ItemSource.ShouldNotBeNull();
            dto.ItemSource.Items.Count.ShouldBe(2);
            dto.Attributes.Count.ShouldBe(2);
        }

        #endregion

        #region LocalizableComboboxItemDto

        [Fact]
        public void Dado_LocalizableComboboxItemDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new LocalizableComboboxItemDto
            {
                Value = "pt-BR",
                DisplayText = "Português (Brasil)"
            };

            dto.Value.ShouldBe("pt-BR");
            dto.DisplayText.ShouldBe("Português (Brasil)");
        }

        #endregion

        #region LocalizableComboboxItemSourceDto

        [Fact]
        public void Dado_LocalizableComboboxItemSourceDto_Quando_DefinirItems_Entao_DeveArmazenar()
        {
            var dto = new LocalizableComboboxItemSourceDto
            {
                Items = new Collection<LocalizableComboboxItemDto>
                {
                    new LocalizableComboboxItemDto { Value = "1" },
                    new LocalizableComboboxItemDto { Value = "2" },
                    new LocalizableComboboxItemDto { Value = "3" }
                }
            };

            dto.Items.Count.ShouldBe(3);
        }

        #endregion
    }
}
