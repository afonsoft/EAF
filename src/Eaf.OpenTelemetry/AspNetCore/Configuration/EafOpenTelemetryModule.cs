using Abp;
using Abp.Modules;

namespace Eaf.AspNetCore.Configuration
{
    /// <summary>
    /// eaf OpenTelemetry module.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class EafOpenTelemetryModule : AbpModule
    {
    }
}