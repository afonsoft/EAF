using Abp.Dependency;
using Abp.TestBase;

namespace Eaf.BlobStoring.Tests
{
    /// <summary>
    /// Base para testes integrados do módulo Eaf.BlobStoring.
    /// </summary>
    public abstract class BlobStoringTestBase : AbpIntegratedTestBase<BlobStoringTestModule>
    {
        /// <summary>
        /// Gerenciador de IoC local dos testes.
        /// </summary>
        protected IIocManager IocManager => LocalIocManager;
    }
}
