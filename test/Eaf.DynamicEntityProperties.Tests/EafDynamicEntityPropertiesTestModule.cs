using Abp.AutoMapper;
using Abp.DynamicEntityProperties;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Eaf.DynamicEntityProperties;
using NSubstitute;

namespace Eaf.DynamicEntityProperties.Tests
{
    /// <summary>
    /// Módulo de testes para o Eaf.DynamicEntityProperties.
    /// </summary>
    [DependsOn(typeof(EafDynamicEntityPropertiesModule), typeof(AbpTestBaseModule))]
    public class EafDynamicEntityPropertiesTestModule : AbpModule
    {
        /// <summary>
        /// Configura o AutoMapper e desabilita a autorização para os testes unitários.
        /// </summary>
        public override void PreInitialize()
        {
#pragma warning disable CS0618
            Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;
#pragma warning restore CS0618
            Configuration.Authorization.IsEnabled = false;
        }

        /// <summary>
        /// Registra os mocks dos gerenciadores do ABP como implementações padrão.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafDynamicEntityPropertiesTestModule).GetAssembly());

            IocManager.IocContainer.Register(
                Component.For<IDynamicPropertyManager>().Instance(Substitute.For<IDynamicPropertyManager>()).IsDefault(),
                Component.For<IDynamicPropertyStore>().Instance(Substitute.For<IDynamicPropertyStore>()).IsDefault(),
                Component.For<IDynamicPropertyValueManager>().Instance(Substitute.For<IDynamicPropertyValueManager>()).IsDefault(),
                Component.For<IDynamicEntityPropertyManager>().Instance(Substitute.For<IDynamicEntityPropertyManager>()).IsDefault(),
                Component.For<IDynamicEntityPropertyValueManager>().Instance(Substitute.For<IDynamicEntityPropertyValueManager>()).IsDefault()
            );
        }
    }
}
