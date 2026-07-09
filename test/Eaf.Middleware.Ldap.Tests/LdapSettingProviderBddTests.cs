using Abp.Configuration;
using Eaf.Middleware.Ldap.Configuration;
using Shouldly;
using NSubstitute;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Eaf.Middleware.Ldap.Tests
{
    public class LdapSettingProviderBddTests
    {
        private static SettingDefinitionProviderContext CriarContexto() =>
            new(Substitute.For<ISettingDefinitionManager>());

        [Fact]
        public void Dado_ProviderLdap_Quando_ObterDefinicoes_Entao_DeveRetornarSeisDefinicoes()
        {
            var provider = new LdapSettingProvider();

            var definitions = provider.GetSettingDefinitions(CriarContexto()).ToList();

            definitions.Count.ShouldBe(6);
            definitions.Any(d => d.Name == LdapSettingNames.IsEnabled).ShouldBeTrue();
            definitions.Any(d => d.Name == LdapSettingNames.ContextType).ShouldBeTrue();
            definitions.Any(d => d.Name == LdapSettingNames.Container).ShouldBeTrue();
            definitions.Any(d => d.Name == LdapSettingNames.Domain).ShouldBeTrue();
            definitions.Any(d => d.Name == LdapSettingNames.UserName).ShouldBeTrue();
            definitions.Any(d => d.Name == LdapSettingNames.Password).ShouldBeTrue();
        }

        [Fact]
        public void Dado_ProviderLdap_Quando_ObterDefinicaoDeSenha_Entao_DeveEstarCriptografada()
        {
            var provider = new LdapSettingProvider();

            var definitions = provider.GetSettingDefinitions(CriarContexto()).ToList();
            var passwordSetting = definitions.First(d => d.Name == LdapSettingNames.Password);

            passwordSetting.IsEncrypted.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ProviderLdap_Quando_Criar_Entao_LocalizationSourceNameDeveSerEafLdap()
        {
            var provider = new LdapSettingProvider();

            var property = typeof(LdapSettingProvider).GetProperty(
                "LocalizationSourceName",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var value = property!.GetValue(provider);

            value.ShouldBe("EafLdap");
        }
    }
}
