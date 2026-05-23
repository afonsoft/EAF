using Eaf.Middleware.Web.Notifications;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Notifications
{
    public class MiddlewareNotificationNamesTests
    {
        [Fact]
        public void Constants()
        {
            MiddlewareNotificationNames.WelcomeToTheApplication.ShouldBe("App.WelcomeToTheApplication");
            MiddlewareNotificationNames.NewUserRegistered.ShouldBe("App.NewUserRegistered");
        }
    }
}
