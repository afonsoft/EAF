using Eaf.Middleware.Localization.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Localization.Dto
{
    public class LanguageDtoBddTests
    {
        #region ApplicationLanguageListDto

        [Fact]
        public void Dado_ApplicationLanguageListDto_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var dto = new ApplicationLanguageListDto
            {
                Name = "pt-BR",
                DisplayName = "Português (Brasil)",
                Icon = "famfamfam-flags br",
                IsDisabled = false,
                TenantId = 1
            };

            dto.Name.ShouldBe("pt-BR");
            dto.DisplayName.ShouldBe("Português (Brasil)");
            dto.Icon.ShouldBe("famfamfam-flags br");
            dto.IsDisabled.ShouldBeFalse();
            dto.TenantId.ShouldBe(1);
        }

        [Fact]
        public void Dado_ApplicationLanguageListDto_SemModificacao_Quando_LastModificationDate_Entao_DeveRetornarCreationTime()
        {
            var creationTime = new DateTime(2024, 1, 15, 10, 30, 0);
            var dto = new ApplicationLanguageListDto
            {
                CreationTime = creationTime,
                LastModificationTime = null
            };

            dto.LastModificationDate.ShouldBe(creationTime);
        }

        [Fact]
        public void Dado_ApplicationLanguageListDto_ComModificacao_Quando_LastModificationDate_Entao_DeveRetornarLastModificationTime()
        {
            var creationTime = new DateTime(2024, 1, 15, 10, 30, 0);
            var modificationTime = new DateTime(2024, 6, 20, 14, 0, 0);
            var dto = new ApplicationLanguageListDto
            {
                CreationTime = creationTime,
                LastModificationTime = modificationTime
            };

            dto.LastModificationDate.ShouldBe(modificationTime);
        }

        #endregion

        #region ApplicationLanguageEditDto

        [Fact]
        public void Dado_ApplicationLanguageEditDto_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var dto = new ApplicationLanguageEditDto
            {
                Id = 1,
                Name = "en",
                Icon = "famfamfam-flags us",
                IsEnabled = true
            };

            dto.Id.ShouldBe(1);
            dto.Name.ShouldBe("en");
            dto.Icon.ShouldBe("famfamfam-flags us");
            dto.IsEnabled.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ApplicationLanguageEditDto_SemId_Quando_Verificar_Entao_IdDeveSerNull()
        {
            var dto = new ApplicationLanguageEditDto { Name = "fr" };

            dto.Id.ShouldBeNull();
        }

        #endregion
    }
}
