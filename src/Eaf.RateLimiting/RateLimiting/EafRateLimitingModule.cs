using Abp;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Eaf.RateLimiting
{
    /// <summary>
    /// Módulo Eaf.RateLimiting — rate limiting por IP/usuário configurável por endpoint.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class EafRateLimitingModule : AbpModule
    {
        /// <inheritdoc />
        public override void PreInitialize()
        {
            IocManager.IocContainer.Register(
                Castle.MicroKernel.Registration.Component
                    .For<EafRateLimitingOptions>()
                    .ImplementedBy<EafRateLimitingOptions>()
                    .LifestyleSingleton());
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafRateLimitingModule).GetAssembly());
        }

        /// <inheritdoc />
        public override void PostInitialize()
        {
            // Registra o IMiddleware no pipeline do host ASP.NET Core, se disponível.
            var services = IocManager.Resolve<IServiceCollection>();
            services.AddTransient<EafRateLimitingMiddleware>();
        }
    }
}
