using Abp;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;

namespace Eaf.Runtime.Caching.Redis
{
    /// <summary>
    /// Este módulo substitui o sistema de cache do EAF por Redis.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class EafRedisCacheModule : AbpModule
    {
        /// <summary>
        /// Inicializa o módulo registrando tipos por convenção.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafRedisCacheModule).GetAssembly());
        }

        /// <summary>
        /// Pré-inicializa o módulo registrando as opções de cache Redis.
        /// </summary>
        public override void PreInitialize()
        {
            if (!IocManager.IsRegistered<EafRedisCacheOptions>())
            {
                IocManager.IocContainer.Register(
                    Component.For<EafRedisCacheOptions, IOptions<RedisCacheOptions>, IOptions<EafRedisCacheOptions>>()
                             .ImplementedBy<EafRedisCacheOptions>()
                             .LifestyleSingleton()
                );
            }
        }
    }
}
