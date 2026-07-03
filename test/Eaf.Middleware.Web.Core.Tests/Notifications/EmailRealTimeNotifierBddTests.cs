using Abp.Net.Mail;
using Abp.Notifications;
using Eaf.Middleware.Authorization.Users;
using Eaf.Notifications;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Notifications
{
    public class EmailRealTimeNotifierBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarInterface_Entao_DeveImplementarIRealTimeNotifier()
        {
            typeof(EmailRealTimeNotifier).GetInterface(nameof(IRealTimeNotifier)).ShouldNotBeNull();
        }
    }
}
