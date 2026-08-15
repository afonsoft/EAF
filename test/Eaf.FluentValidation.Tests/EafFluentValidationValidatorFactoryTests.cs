using Abp;
using Abp.Dependency;
using Eaf.FluentValidation.Tests.SampleValidators;
using Shouldly;
using Xunit;

namespace Eaf.FluentValidation.Tests
{
    public class EafFluentValidationValidatorFactoryTests
    {
        [Fact]
        public void Dado_TipoComValidatorRegistrado_Quando_Resolver_Entao_RetornaValidator()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var factory = bootstrapper.IocManager.Resolve<EafFluentValidationValidatorFactory>();
            var validator = factory.GetValidator(typeof(CreateUserInput));

            validator.ShouldNotBeNull();
            validator.ShouldBeAssignableTo<global::FluentValidation.IValidator<CreateUserInput>>();
        }

        [Fact]
        public void Dado_TipoSemValidatorRegistrado_Quando_Resolver_Entao_RetornaNulo()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var factory = bootstrapper.IocManager.Resolve<EafFluentValidationValidatorFactory>();
            var validator = factory.GetValidator(typeof(object));

            validator.ShouldBeNull();
        }

        private static AbpBootstrapper CriarBootstrapper()
        {
            return AbpBootstrapper.Create<EafFluentValidationTestModule>(options =>
            {
                options.IocManager = new IocManager();
            });
        }
    }
}
