using Eaf.WebHooks;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.WebHooks
{
    public class EafWebHookNamesTests
    {
        [Fact]
        public void NewUserRegistered_IsConstant()
        {
            EafWebHookNames.NewUserRegistered.ShouldBe("WebHook.NewUserRegistered");
        }
    }
}
