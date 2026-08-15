using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Eaf.FluentValidation.Tests.SampleValidators;

namespace Eaf.FluentValidation.Tests
{
    /// <summary>
    /// Módulo de teste para Eaf.FluentValidation.
    /// </summary>
    [DependsOn(typeof(EafFluentValidationModule))]
    public class EafFluentValidationTestModule : AbpModule
    {
        /// <summary>
        /// Inicializa o módulo de teste registrando tipos por convenção.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafFluentValidationTestModule).GetAssembly());
        }

        /// <summary>
        /// Adiciona o assembly de testes às opções de escaneamento de validators.
        /// </summary>
        public override void PreInitialize()
        {
            var options = IocManager.Resolve<EafFluentValidationOptions>();
            options.ValidatorAssemblies.Add(typeof(EafFluentValidationTestModule).GetAssembly());
        }
    }
}
