using Abp.Dependency;
using Abp.TestBase;

namespace Eaf.Notifications.Sms.Tests
{
    /// <summary>
    /// Classe base para testes integrados de Eaf.Notifications.Sms.
    /// </summary>
    public abstract class EafNotificationsSmsTestBase : AbpIntegratedTestBase<EafNotificationsSmsTestModule>
    {
        /// <summary>
        /// Gerenciador de IoC local dos testes.
        /// </summary>
        protected IIocManager IocManager => LocalIocManager;
    }
}
