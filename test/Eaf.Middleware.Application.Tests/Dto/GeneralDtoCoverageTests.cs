using Abp.Runtime.Validation;
using Eaf.Middleware.Dto;
using Eaf.Middleware.Editions.Dto;
using Shouldly;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Dto
{
    public class GeneralDtoCoverageTests
    {
        [Fact]
        public void FileDto_DefaultCtor_ShouldLeaveNull()
        {
            var dto = new FileDto();
            dto.FileName.ShouldBeNull();
            dto.FileToken.ShouldBeNull();
            dto.FileType.ShouldBeNull();
        }

        [Fact]
        public void FileDto_Ctor_ShouldAssign()
        {
            var dto = new FileDto("name", "pdf");
            dto.FileName.ShouldBe("name");
            dto.FileType.ShouldBe("pdf");
            dto.FileToken.ShouldNotBeNullOrWhiteSpace();
            dto.FileToken.Length.ShouldBe(32);
        }

        [Fact]
        public void FeatureInputTypeDto_ShouldSet()
        {
            var validator = new NumericValueValidator(1, 10);
            var attrs = new Dictionary<string, object> { ["k"] = 1 };
            var source = new LocalizableComboboxItemSourceDto();
            var dto = new FeatureInputTypeDto
            {
                Attributes = attrs,
                ItemSource = source,
                Name = "n",
                Validator = validator
            };
            dto.Attributes.ShouldBe(attrs);
            dto.ItemSource.ShouldBe(source);
            dto.Name.ShouldBe("n");
            dto.Validator.ShouldBe(validator);
        }

        [Fact]
        public void FlatFeatureDto_ShouldSet()
        {
            var it = new FeatureInputTypeDto();
            var dto = new FlatFeatureDto
            {
                DefaultValue = "d",
                Description = "desc",
                DisplayName = "dn",
                InputType = it,
                Name = "n",
                ParentName = "p"
            };
            dto.DefaultValue.ShouldBe("d");
            dto.Description.ShouldBe("desc");
            dto.DisplayName.ShouldBe("dn");
            dto.InputType.ShouldBe(it);
            dto.Name.ShouldBe("n");
            dto.ParentName.ShouldBe("p");
        }

        [Fact]
        public void FlatFeatureSelectDto_ShouldSet()
        {
            var dto = new FlatFeatureSelectDto
            {
                DefaultValue = "d",
                Description = "desc",
                DisplayName = "dn",
                InputType = null,
                Name = "n",
                ParentName = "p",
                TextHtmlColor = "#000"
            };
            dto.DefaultValue.ShouldBe("d");
            dto.Description.ShouldBe("desc");
            dto.DisplayName.ShouldBe("dn");
            dto.InputType.ShouldBeNull();
            dto.Name.ShouldBe("n");
            dto.ParentName.ShouldBe("p");
            dto.TextHtmlColor.ShouldBe("#000");
        }

        [Fact]
        public void LocalizableComboboxItemDto_ShouldSet()
        {
            var dto = new LocalizableComboboxItemDto { DisplayText = "t", Value = "v" };
            dto.DisplayText.ShouldBe("t");
            dto.Value.ShouldBe("v");
        }

        [Fact]
        public void LocalizableComboboxItemSourceDto_ShouldSet()
        {
            var items = new Collection<LocalizableComboboxItemDto>();
            var dto = new LocalizableComboboxItemSourceDto { Items = items };
            dto.Items.ShouldBe(items);
        }

        [Fact]
        public void PagedAndFilteredInputDto_Defaults()
        {
            var dto = new PagedAndFilteredInputDto();
            dto.Filter.ShouldBe("");
            dto.MaxResultCount.ShouldBe(MiddlewareAppConsts.DefaultPageSize);
            dto.SkipCount.ShouldBe(0);

            dto.Filter = "f";
            dto.MaxResultCount = 25;
            dto.SkipCount = 5;
            dto.Filter.ShouldBe("f");
            dto.MaxResultCount.ShouldBe(25);
            dto.SkipCount.ShouldBe(5);
        }

        [Fact]
        public void PagedAndSortedInputDto_Defaults()
        {
            var dto = new PagedAndSortedInputDto();
            dto.Sorting.ShouldBe("");
            dto.MaxResultCount.ShouldBe(MiddlewareAppConsts.DefaultPageSize);
            dto.Sorting = "Name";
            dto.Sorting.ShouldBe("Name");
        }

        [Fact]
        public void PagedInputDto_Defaults()
        {
            var dto = new PagedInputDto();
            dto.MaxResultCount.ShouldBe(MiddlewareAppConsts.DefaultPageSize);
            dto.SkipCount.ShouldBe(0);
        }

        [Fact]
        public void PagedSortedAndFilteredInputDto_Defaults()
        {
            var dto = new PagedSortedAndFilteredInputDto();
            dto.Filter.ShouldBe("");
            dto.Sorting.ShouldBe("");
            dto.Filter = "x";
            dto.Filter.ShouldBe("x");
        }
    }
}
