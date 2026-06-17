using Abp.Localization;
using Abp.Localization.Sources;
using Eaf.Middleware.Localization;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using System;
using System.Globalization;
using Xunit;

namespace Eaf.Middleware.Tests.LocalizationTests
{
    /// <summary>
    /// Testes BDD para MiddlewareLocalizationHelper seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class MiddlewareLocalizationHelperBddTests
    {
        [Fact]
        public void Dado_ManagerNulo_Quando_Localize_Entao_DeveRetornarChave()
        {
            var result = MiddlewareLocalizationHelper.Localize(null, "TestKey");
            result.ShouldBe("TestKey");
        }

        [Fact]
        public void Dado_ChaveNulaOuVazia_Quando_Localize_Entao_DeveRetornarChave()
        {
            var manager = Substitute.For<ILocalizationManager>();
            MiddlewareLocalizationHelper.Localize(manager, null).ShouldBeNull();
            MiddlewareLocalizationHelper.Localize(manager, "").ShouldBe("");
        }

        [Fact]
        public void Dado_SourceComChave_Quando_Localize_Entao_DeveRetornarTraducao()
        {
            var manager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.GetStringOrNull("LoginFailed", Arg.Any<CultureInfo>()).Returns("Falha no Login");
            manager.GetSource("EafCore").Returns(source);

            var result = MiddlewareLocalizationHelper.Localize(manager, "LoginFailed");
            result.ShouldBe("Falha no Login");
        }

        [Fact]
        public void Dado_PrimeiroSourceSemChave_Quando_Localize_Entao_DeveBuscarNoProximo()
        {
            var manager = Substitute.For<ILocalizationManager>();

            var eafSource = Substitute.For<ILocalizationSource>();
            eafSource.GetStringOrNull("AbpKey", Arg.Any<CultureInfo>()).Returns((string)null);
            manager.GetSource("EafCore").Returns(eafSource);

            var abpSource = Substitute.For<ILocalizationSource>();
            abpSource.GetStringOrNull("AbpKey", Arg.Any<CultureInfo>()).Returns("ABP Valor");
            manager.GetSource("Abp").Returns(abpSource);

            var result = MiddlewareLocalizationHelper.Localize(manager, "AbpKey");
            result.ShouldBe("ABP Valor");
        }

        [Fact]
        public void Dado_NenhumSourceComChave_Quando_Localize_Entao_DeveRetornarChave()
        {
            var manager = Substitute.For<ILocalizationManager>();
            manager.GetSource(Arg.Any<string>()).Throws(new Exception("Source not found"));

            var result = MiddlewareLocalizationHelper.Localize(manager, "MissingKey");
            result.ShouldBe("MissingKey");
        }

        [Fact]
        public void Dado_SourceComChave_Quando_LocalizeComArgs_Entao_DeveFormatar()
        {
            var manager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.GetStringOrNull("WelcomeMessage", Arg.Any<CultureInfo>()).Returns("Olá, {0}!");
            manager.GetSource("EafCore").Returns(source);

            var result = MiddlewareLocalizationHelper.Localize(manager, "WelcomeMessage", "João");
            result.ShouldBe("Olá, João!");
        }

        [Fact]
        public void Dado_SourceComChave_Quando_LocalizeComCultura_Entao_DeveUsarCultura()
        {
            var ptBR = new CultureInfo("pt-BR");
            var manager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.GetStringOrNull("Save", ptBR).Returns("Salvar");
            manager.GetSource("EafCore").Returns(source);

            var result = MiddlewareLocalizationHelper.Localize(manager, "Save", ptBR);
            result.ShouldBe("Salvar");
        }

        [Fact]
        public void Dado_SourceComChave_Quando_LocalizeComCulturaEArgs_Entao_DeveFormatarComCultura()
        {
            var ptBR = new CultureInfo("pt-BR");
            var manager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.GetStringOrNull("UserCount", ptBR).Returns("{0} usuários ativos");
            manager.GetSource("EafCore").Returns(source);

            var result = MiddlewareLocalizationHelper.Localize(manager, "UserCount", ptBR, 42);
            result.ShouldBe("42 usuários ativos");
        }

        [Fact]
        public void Dado_MiddlewareLocalizationHelper_Quando_VerificarSourceNames_Entao_DeveConterSourcesCorretos()
        {
            MiddlewareLocalizationHelper.SourceNames.Length.ShouldBe(6);
            MiddlewareLocalizationHelper.SourceNames[0].ShouldBe("EafCore");
            MiddlewareLocalizationHelper.DefaultSourceName.ShouldBe("EafCore");
        }
    }
}
