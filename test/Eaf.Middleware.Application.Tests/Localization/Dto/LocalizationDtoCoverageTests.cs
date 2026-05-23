using Abp.Application.Services.Dto;
using Eaf.Middleware.Localization.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Localization.Dto
{
    public class LocalizationDtoCoverageTests
    {
        [Fact]
        public void ApplicationLanguageEditDto_ShouldSet()
        {
            var dto = new ApplicationLanguageEditDto
            {
                Icon = "flag-br",
                Id = 1,
                IsEnabled = true,
                Name = "pt-BR"
            };
            dto.Icon.ShouldBe("flag-br");
            dto.Id.ShouldBe(1);
            dto.IsEnabled.ShouldBeTrue();
            dto.Name.ShouldBe("pt-BR");
        }

        [Fact]
        public void ApplicationLanguageListDto_LastModificationDate_UsesLastModificationTime()
        {
            var dto = new ApplicationLanguageListDto
            {
                CreationTime = new DateTime(2020, 1, 1),
                LastModificationTime = new DateTime(2021, 2, 3),
                DisplayName = "Portuguese",
                Icon = "flag",
                IsDisabled = false,
                Name = "pt-BR",
                TenantId = 5
            };
            dto.LastModificationDate.ShouldBe(new DateTime(2021, 2, 3));
            dto.DisplayName.ShouldBe("Portuguese");
            dto.Icon.ShouldBe("flag");
            dto.IsDisabled.ShouldBeFalse();
            dto.Name.ShouldBe("pt-BR");
            dto.TenantId.ShouldBe(5);
        }

        [Fact]
        public void ApplicationLanguageListDto_LastModificationDate_FallsBackToCreationTime()
        {
            var dto = new ApplicationLanguageListDto
            {
                CreationTime = new DateTime(2020, 1, 1),
                LastModificationTime = null
            };
            dto.LastModificationDate.ShouldBe(new DateTime(2020, 1, 1));
        }

        [Fact]
        public void CreateOrUpdateLanguageInput_ShouldSet()
        {
            var lang = new ApplicationLanguageEditDto { Name = "en" };
            var dto = new CreateOrUpdateLanguageInput { Language = lang };
            dto.Language.ShouldBe(lang);
        }

        [Fact]
        public void GetLanguageForEditOutput_Defaults()
        {
            var dto = new GetLanguageForEditOutput();
            dto.Flags.ShouldNotBeNull();
            dto.LanguageNames.ShouldNotBeNull();
            dto.Language.ShouldBeNull();

            dto.Flags.Add(new ComboboxItemDto("k", "v"));
            dto.LanguageNames.Add(new ComboboxItemDto("a", "b"));
            dto.Language = new ApplicationLanguageEditDto();
            dto.Flags.Count.ShouldBe(1);
            dto.LanguageNames.Count.ShouldBe(1);
            dto.Language.ShouldNotBeNull();
        }

        [Fact]
        public void GetLanguagesInput_Normalize_DefaultsToName()
        {
            var dto = new GetLanguagesInput();
            dto.Normalize();
            dto.Sorting.ShouldBe("Name");
        }

        [Fact]
        public void GetLanguagesInput_Normalize_PreservesExistingSort()
        {
            var dto = new GetLanguagesInput { Sorting = "DisplayName" };
            dto.Normalize();
            dto.Sorting.ShouldBe("DisplayName");
        }

        [Fact]
        public void GetLanguagesOutput_DefaultCtor()
        {
            var dto = new GetLanguagesOutput();
            dto.DefaultLanguageName.ShouldBeNull();
            dto.Items.ShouldNotBeNull();
        }

        [Fact]
        public void GetLanguagesOutput_Ctor_AssignsItems()
        {
            var list = new List<ApplicationLanguageListDto>
            {
                new ApplicationLanguageListDto { Name = "pt-BR" }
            };
            var dto = new GetLanguagesOutput(list, "pt-BR");
            dto.DefaultLanguageName.ShouldBe("pt-BR");
            dto.Items.Count.ShouldBe(1);
        }

        [Fact]
        public void GetLanguageTextsInput_Normalize_DefaultsToALL()
        {
            var dto = new GetLanguageTextsInput();
            dto.Normalize();
            dto.TargetValueFilter.ShouldBe("ALL");
        }

        [Fact]
        public void GetLanguageTextsInput_Normalize_PreservesExistingFilter()
        {
            var dto = new GetLanguageTextsInput { TargetValueFilter = "EMPTY" };
            dto.Normalize();
            dto.TargetValueFilter.ShouldBe("EMPTY");
        }

        [Fact]
        public void LanguageTextListDto_ShouldSet()
        {
            var dto = new LanguageTextListDto { BaseValue = "b", Key = "k", TargetValue = "t" };
            dto.BaseValue.ShouldBe("b");
            dto.Key.ShouldBe("k");
            dto.TargetValue.ShouldBe("t");
        }

        [Fact]
        public void SetDefaultLanguageInput_ShouldSet()
        {
            var dto = new SetDefaultLanguageInput { Name = "pt-BR" };
            dto.Name.ShouldBe("pt-BR");
        }

        [Fact]
        public void UpdateLanguageTextInput_ShouldSet()
        {
            var dto = new UpdateLanguageTextInput
            {
                Key = "k",
                LanguageName = "en",
                SourceName = "src",
                Value = "v"
            };
            dto.Key.ShouldBe("k");
            dto.LanguageName.ShouldBe("en");
            dto.SourceName.ShouldBe("src");
            dto.Value.ShouldBe("v");
        }
    }
}
