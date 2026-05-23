using Abp;
using Abp.Modules;

namespace Eaf.Castle.Logging.SerilogIntegration
{
    /// <summary>
    /// eaf Castle Serilog module.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class EafCastleSerilogModule : AbpModule
    {
    }
}