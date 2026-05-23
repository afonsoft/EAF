using Abp.Configuration;
using Eaf.Middleware.Timing.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Net.Timing.Dto
{
    public class TimingDtoTests
    {
        [Fact]
        public void GetTimezoneComboboxItemsInput_ShouldSet()
        {
            var dto = new GetTimezoneComboboxItemsInput
            {
                DefaultTimezoneScope = SettingScopes.Tenant,
                SelectedTimezoneId = "UTC"
            };
            dto.DefaultTimezoneScope.ShouldBe(SettingScopes.Tenant);
            dto.SelectedTimezoneId.ShouldBe("UTC");
        }

        [Fact]
        public void GetTimezonesInput_Defaults()
        {
            var dto = new GetTimezonesInput();
            dto.DefaultTimezoneScope.ShouldBe(SettingScopes.Application);
        }
    }
}
