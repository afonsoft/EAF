using Abp.Dependency;
using Abp.TestBase;

namespace Eaf.DynamicEntityProperties.Tests
{
    /// <summary>
    /// Base para testes integrados do módulo Eaf.DynamicEntityProperties.
    /// </summary>
    public abstract class EafDynamicEntityPropertiesTestBase : AbpIntegratedTestBase<EafDynamicEntityPropertiesTestModule>
    {
        /// <summary>
        /// Gerenciador de IoC local dos testes.
        /// </summary>
        protected IIocManager IocManager => LocalIocManager;
    }
}
