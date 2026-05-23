using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Eaf.Middleware;

namespace Eaf
{
    [DependsOn(
        typeof(MiddlewareApplicationModule),
        typeof(AbpTestBaseModule)
    )]
    public class EafMiddlewareApplicationTestModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafMiddlewareApplicationTestModule).GetAssembly());
        }
    }
}
