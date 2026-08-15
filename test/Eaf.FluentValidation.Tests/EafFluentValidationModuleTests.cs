using Abp;
using Abp.Dependency;
using Abp.Runtime.Validation;
using Abp.Runtime.Validation.Interception;
using Eaf.FluentValidation.Tests.SampleValidators;
using Shouldly;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Eaf.FluentValidation.Tests
{
    public class EafFluentValidationModuleTests
    {
        [Fact]
        public void Dado_EafFluentValidationModule_Quando_Inicializar_Entao_Servicos_E_Opcoes_Sao_Registrados()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var container = bootstrapper.IocManager;
            container.Resolve<EafFluentValidationOptions>().ShouldNotBeNull();
            container.Resolve<EafFluentValidationValidatorFactory>().ShouldNotBeNull();
            container.Resolve<EafFluentValidationMethodParameterValidator>().ShouldNotBeNull();
            container.Resolve<global::FluentValidation.IValidator<CreateUserInput>>().ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ValidadorRegistrado_Quando_ResolverPeloTipo_Entao_RetornaImplementacaoCorreta()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var container = bootstrapper.IocManager;
            var validator = container.Resolve<global::FluentValidation.IValidator<CreateUserInput>>();

            validator.ShouldNotBeNull();
            validator.ShouldBeAssignableTo<CreateUserInputValidator>();
        }

        [Fact]
        public void Dado_MethodInvocationValidator_Quando_ExecutarValidacao_Entao_IncluiErrosFluentValidation()
        {
            using var bootstrapper = CriarBootstrapper();
            bootstrapper.Initialize();

            var invocationValidator = bootstrapper.IocManager.Resolve<MethodInvocationValidator>();
            var method = typeof(TestService).GetMethod(nameof(TestService.Create), BindingFlags.Public | BindingFlags.Instance);
            var input = new CreateUserInput
            {
                Name = "EAF",
                Email = "invalid",
                Password = "12345678"
            };

            invocationValidator.Initialize(method, new object[] { input });

            var exception = Should.Throw<AbpValidationException>(() => invocationValidator.Validate());
            exception.ValidationErrors.ShouldContain(e => e.MemberNames.Any(m => m == "Email"));
        }

        private static AbpBootstrapper CriarBootstrapper()
        {
            return AbpBootstrapper.Create<EafFluentValidationTestModule>(options =>
            {
                options.IocManager = new IocManager();
            });
        }

        public class TestService
        {
            public void Create(CreateUserInput input)
            {
                _ = input;
            }
        }
    }
}
