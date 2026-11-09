using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Eaf.Middleware.Net.Emailing;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Net.Emailing
{
    /// <summary>
    /// Testes BDD para <see cref="EmailTemplateManager"/> seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class EmailTemplateManagerBddTests
    {
        private const string DefaultBody = "<html>{EMAIL_TITLE}{EMAIL_SUB_TITLE}{EMAIL_BODY}{THIS_YEAR}</html>";

        private static EmailTemplateManager CriarSut(IRepository<EmailTemplateOverride> overrides)
        {
            var provider = Substitute.For<IEmailTemplateProvider>();
            provider.GetDefaultTemplate(Arg.Any<int?>()).Returns(DefaultBody);

            var uowProvider = Substitute.For<ICurrentUnitOfWorkProvider>();
            uowProvider.Current.Returns(Substitute.For<IUnitOfWork>());

            return new EmailTemplateManager(
                provider,
                new EmailTemplateDefinitionProvider(),
                overrides,
                uowProvider);
        }

        private static IRepository<EmailTemplateOverride> RepositorioCom(params EmailTemplateOverride[] rows)
        {
            var repo = Substitute.For<IRepository<EmailTemplateOverride>>();
            repo.GetAllListAsync(Arg.Any<Expression<Func<EmailTemplateOverride, bool>>>())
                .Returns(ci =>
                {
                    var predicate = ci.Arg<Expression<Func<EmailTemplateOverride, bool>>>().Compile();
                    return Task.FromResult((System.Collections.Generic.List<EmailTemplateOverride>)rows.Where(predicate).ToList());
                });
            return repo;
        }

        [Fact]
        public async Task Dado_OverrideCulturaExata_Quando_Resolver_Entao_DeveRetornarOverride()
        {
            var repo = RepositorioCom(
                new EmailTemplateOverride { TenantId = 1, Name = "Default", Culture = "pt-BR", Body = "<b>PT {EMAIL_BODY}</b>" },
                new EmailTemplateOverride { TenantId = 1, Name = "Default", Culture = string.Empty, Body = "<b>EN {EMAIL_BODY}</b>" });
            var sut = CriarSut(repo);

            var body = await sut.ResolveBodyAsync(1, "Default", "pt-BR");

            body.ShouldBe("<b>PT {EMAIL_BODY}</b>");
        }

        [Fact]
        public async Task Dado_SoOverrideNeutro_Quando_ResolverOutraCultura_Entao_DeveCairNaNeutra()
        {
            var repo = RepositorioCom(
                new EmailTemplateOverride { TenantId = 1, Name = "Default", Culture = string.Empty, Body = "<b>N {EMAIL_BODY}</b>" });
            var sut = CriarSut(repo);

            var body = await sut.ResolveBodyAsync(1, "Default", "es");

            body.ShouldBe("<b>N {EMAIL_BODY}</b>");
        }

        [Fact]
        public async Task Dado_SemOverride_Quando_Resolver_Entao_DeveCairNoPadrao()
        {
            var sut = CriarSut(RepositorioCom());

            var body = await sut.ResolveBodyAsync(2, "Default", "pt-BR");

            body.ShouldBe(DefaultBody);
        }

        [Fact]
        public async Task Dado_OverrideDeOutroTenant_Quando_Resolver_Entao_DeveCairNoPadrao()
        {
            var repo = RepositorioCom(
                new EmailTemplateOverride { TenantId = 5, Name = "Default", Culture = string.Empty, Body = "<b>X {EMAIL_BODY}</b>" });
            var sut = CriarSut(repo);

            var body = await sut.ResolveBodyAsync(1, "Default", "en");

            body.ShouldBe(DefaultBody);
        }

        [Fact]
        public void Dado_CorpoValido_Quando_Validar_Entao_NaoDeveLancar()
        {
            var sut = CriarSut(RepositorioCom());

            Should.NotThrow(() => sut.ValidateBody("Default", "<p>{EMAIL_TITLE}{EMAIL_BODY}</p>"));
        }

        [Fact]
        public void Dado_PlaceholderDesconhecido_Quando_Validar_Entao_DeveLancarUserFriendly()
        {
            var sut = CriarSut(RepositorioCom());

            Should.Throw<UserFriendlyException>(() => sut.ValidateBody("Default", "<p>{EMAIL_BODY}{HACK}</p>"));
        }

        [Fact]
        public void Dado_PlaceholderObrigatorioAusente_Quando_Validar_Entao_DeveLancarUserFriendly()
        {
            var sut = CriarSut(RepositorioCom());

            Should.Throw<UserFriendlyException>(() => sut.ValidateBody("Default", "<p>{EMAIL_TITLE}</p>"));
        }

        [Fact]
        public void Dado_TemplateDesconhecido_Quando_Validar_Entao_DeveLancarUserFriendly()
        {
            var sut = CriarSut(RepositorioCom());

            Should.Throw<UserFriendlyException>(() => sut.ValidateBody("DoesNotExist", "{EMAIL_BODY}"));
        }

        [Fact]
        public void Dado_CorpoComPlaceholders_Quando_RenderizarPreview_Entao_DeveSubstituir()
        {
            var sut = CriarSut(RepositorioCom());

            var html = sut.RenderPreview("Default", "<title>{EMAIL_TITLE}</title><div>{EMAIL_BODY}</div>");

            html.ShouldContain("Sample email title");
            html.ShouldContain("Sample email body content");
            html.ShouldNotContain("{EMAIL_TITLE}");
        }
    }
}
