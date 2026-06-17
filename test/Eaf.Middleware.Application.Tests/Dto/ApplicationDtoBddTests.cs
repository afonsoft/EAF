using Eaf.Middleware.Dto;
using Eaf.Middleware.Editions.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.Dto
{
    /// <summary>
    /// Testes BDD para DTOs base da Aplicação seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ApplicationDtoBddTests
    {
        #region MiddlewareAppConsts

        [Fact]
        public void Dado_MiddlewareAppConsts_Quando_Verificar_Entao_DevemTerValoresCorretos()
        {
            MiddlewareAppConsts.DefaultPageSize.ShouldBe(30);
            MiddlewareAppConsts.MaxPageSize.ShouldBe(300000);
            MiddlewareAppConsts.LocalizationSourceName.ShouldBe("EafCore");
            MiddlewareAppConsts.SystemProvider.ShouldBe("System");
            MiddlewareAppConsts.ThemeDefault.ShouldBe("default");
            MiddlewareAppConsts.Theme2.ShouldBe("theme2");
            MiddlewareAppConsts.Theme3.ShouldBe("theme3");
            MiddlewareAppConsts.Theme4.ShouldBe("theme4");
            MiddlewareAppConsts.MaxProfilPictureBytesUserFriendlyValue.ShouldBe(5);
            MiddlewareAppConsts.ResizedMaxProfilPictureBytesUserFriendlyValue.ShouldBe(1024);
        }

        #endregion

        #region FileDto

        [Fact]
        public void Dado_FileDtoPadrao_Quando_Criar_Entao_DeveInicializarVazio()
        {
            var dto = new FileDto();
            dto.FileName.ShouldBeNull();
            dto.FileToken.ShouldBeNull();
        }

        [Fact]
        public void Dado_FileDto_Quando_CriarComNomeETipo_Entao_DeveGerarToken()
        {
            var dto = new FileDto("report.xlsx", "application/vnd.ms-excel");
            dto.FileName.ShouldBe("report.xlsx");
            dto.FileType.ShouldBe("application/vnd.ms-excel");
            dto.FileToken.ShouldNotBeNullOrEmpty();
            dto.FileToken.Length.ShouldBe(32);
        }

        [Fact]
        public void Dado_DoisFileDto_Quando_CriarComMesmoNome_Entao_TokensDevemSerDiferentes()
        {
            var dto1 = new FileDto("file.csv", "text/csv");
            var dto2 = new FileDto("file.csv", "text/csv");
            dto1.FileToken.ShouldNotBe(dto2.FileToken);
        }

        #endregion

        #region PagedInputDto

        [Fact]
        public void Dado_PagedInputDto_Quando_Criar_Entao_MaxResultCountDeveSerDefaultPageSize()
        {
            var dto = new PagedInputDto();
            dto.MaxResultCount.ShouldBe(MiddlewareAppConsts.DefaultPageSize);
            dto.SkipCount.ShouldBe(0);
        }

        [Fact]
        public void Dado_PagedInputDto_Quando_DefinirValores_Entao_DeveArmazenar()
        {
            var dto = new PagedInputDto { MaxResultCount = 50, SkipCount = 10 };
            dto.MaxResultCount.ShouldBe(50);
            dto.SkipCount.ShouldBe(10);
        }

        #endregion

        #region PagedAndSortedInputDto

        [Fact]
        public void Dado_PagedAndSortedInputDto_Quando_Criar_Entao_SortingDeveSerVazio()
        {
            var dto = new PagedAndSortedInputDto();
            dto.Sorting.ShouldBe("");
            dto.MaxResultCount.ShouldBe(MiddlewareAppConsts.DefaultPageSize);
        }

        #endregion

        #region PagedAndFilteredInputDto

        [Fact]
        public void Dado_PagedAndFilteredInputDto_Quando_Criar_Entao_FilterDeveSerVazio()
        {
            var dto = new PagedAndFilteredInputDto();
            dto.Filter.ShouldBe("");
            dto.MaxResultCount.ShouldBe(MiddlewareAppConsts.DefaultPageSize);
        }

        #endregion

        #region PagedSortedAndFilteredInputDto

        [Fact]
        public void Dado_PagedSortedAndFilteredInputDto_Quando_Criar_Entao_FilterEsortingDevemSerVazios()
        {
            var dto = new PagedSortedAndFilteredInputDto();
            dto.Filter.ShouldBe("");
            dto.Sorting.ShouldBe("");
        }

        #endregion

        #region FlatFeatureDto

        [Fact]
        public void Dado_FlatFeatureDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new FlatFeatureDto
            {
                Name = "MaxUsers",
                DisplayName = "Máximo de Usuários",
                Description = "Limite de usuários",
                DefaultValue = "10",
                ParentName = "TenantFeatures"
            };

            dto.Name.ShouldBe("MaxUsers");
            dto.DisplayName.ShouldBe("Máximo de Usuários");
            dto.Description.ShouldBe("Limite de usuários");
            dto.DefaultValue.ShouldBe("10");
            dto.ParentName.ShouldBe("TenantFeatures");
        }

        #endregion

        #region FeatureInputTypeDto

        [Fact]
        public void Dado_FeatureInputTypeDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new FeatureInputTypeDto
            {
                Name = "CheckboxInputType",
                Attributes = new Dictionary<string, object> { { "checked", true } }
            };

            dto.Name.ShouldBe("CheckboxInputType");
            dto.Attributes.ShouldContainKey("checked");
        }

        #endregion

        #region LocalizableComboboxItemDto

        [Fact]
        public void Dado_LocalizableComboboxItemDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new LocalizableComboboxItemDto
            {
                Value = "1",
                DisplayText = "Opção 1"
            };

            dto.Value.ShouldBe("1");
            dto.DisplayText.ShouldBe("Opção 1");
        }

        [Fact]
        public void Dado_LocalizableComboboxItemSourceDto_Quando_DefinirItems_Entao_DeveArmazenar()
        {
            var source = new LocalizableComboboxItemSourceDto
            {
                Items = new System.Collections.ObjectModel.Collection<LocalizableComboboxItemDto>
                {
                    new LocalizableComboboxItemDto { Value = "a", DisplayText = "A" },
                    new LocalizableComboboxItemDto { Value = "b", DisplayText = "B" }
                }
            };

            source.Items.Count.ShouldBe(2);
        }

        #endregion
    }
}
