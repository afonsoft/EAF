using Abp;
using Abp.Localization;
using Shouldly;
using System.Globalization;
using System.Threading;
using Xunit;

namespace Eaf.Middleware.Localization
{
    public class SimpleLocalization_Tests : EafMiddlewareTestBase
    {
        [Theory]
        [InlineData("en")]
        [InlineData("en-US")]
        [InlineData("en-GB")]
        public void Test1(string cultureName)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);

            Resolve<ILocalizationManager>().GetString(AbpConsts.LocalizationSourceName, "Identity.UserNotInRole")
            .ShouldBe("[Identity.User not in role]");
        }
    }
}