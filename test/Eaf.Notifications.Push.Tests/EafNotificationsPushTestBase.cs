using Abp.Dependency;
using Abp.TestBase;

namespace Eaf.Notifications.Push.Tests
{
    /// <summary>
    /// Classe base para testes integrados de Eaf.Notifications.Push.
    /// </summary>
    public abstract class EafNotificationsPushTestBase : AbpIntegratedTestBase<EafNotificationsPushTestModule>
    {
        /// <summary>
        /// Gerenciador de IoC local dos testes.
        /// </summary>
        protected IIocManager IocManager => LocalIocManager;
    }
}
