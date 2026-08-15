using Abp;
using Abp.AutoMapper;
using Abp.DynamicEntityProperties;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Eaf.DynamicEntityProperties.Application;
using Eaf.DynamicEntityProperties.Authorization;
using System.Reflection;

namespace Eaf.DynamicEntityProperties
{
    /// <summary>
    /// EAF module that exposes ABP dynamic entity properties as application services
    /// and wires the required permissions and AutoMapper mappings.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule), typeof(AbpAutoMapperModule))]
    public class EafDynamicEntityPropertiesModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<EafDynamicEntityPropertiesAuthorizationProvider>();
            Configuration.Modules.AbpAutoMapper().Configurators.Add(DynamicEntityPropertiesDtoMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafDynamicEntityPropertiesModule).GetAssembly());
        }
    }
}
