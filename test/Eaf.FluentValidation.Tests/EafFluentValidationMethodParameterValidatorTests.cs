using Abp;
using Abp.Dependency;
using Abp.Runtime.Validation.Interception;
using Eaf.FluentValidation.Tests.SampleValidators;
using Shouldly;
using System.Linq;
using Xunit;

namespace Eaf.FluentValidation.Tests
{
    public class EafFluentValidationMethodParameterValidatorTests
    {
        [Fact]
        public void Dado_InputInvalido_Quando_Validar_Entao_RetornaErrosDoFluentValidation()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var validator = bootstrapper.IocManager.Resolve<EafFluentValidationMethodParameterValidator>();
            var input = new CreateUserInput
            {
                Name = "Test",
                Email = "invalid",
                Password = "12345678"
            };

            var result = validator.Validate(input);

            result.ShouldNotBeEmpty();
            result.Any(r => r.MemberNames.Contains("Email")).ShouldBeTrue();
        }

        [Fact]
        public void Dado_InputValido_Quando_Validar_Entao_RetornaListaVazia()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var validator = bootstrapper.IocManager.Resolve<EafFluentValidationMethodParameterValidator>();
            var input = new CreateUserInput
            {
                Name = "Test",
                Email = "test@example.com",
                Password = "12345678"
            };

            var result = validator.Validate(input);

            result.ShouldBeEmpty();
        }

        [Fact]
        public void Dado_InputNulo_Quando_Validar_Entao_RetornaListaVazia()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var validator = bootstrapper.IocManager.Resolve<EafFluentValidationMethodParameterValidator>();

            var result = validator.Validate(null);

            result.ShouldBeEmpty();
        }

        [Fact]
        public void Dado_TipoSemValidator_Quando_Validar_Entao_RetornaListaVazia()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var validator = bootstrapper.IocManager.Resolve<EafFluentValidationMethodParameterValidator>();

            var result = validator.Validate(new { Id = 1 });

            result.ShouldBeEmpty();
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
