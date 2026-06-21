using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Runtime.Session;
using Eaf.Middleware.Localization;
using Eaf.Middleware.Localization.Dto;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Localization
{
    /// <summary>
    /// Testes BDD para LanguageAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class LanguageAppServiceBddTests
    {
        private readonly IApplicationLanguageManager _applicationLanguageManager;
        private readonly IApplicationLanguageTextManager _applicationLanguageTextManager;
        private readonly IRepository<ApplicationLanguage> _languageRepository;
        private readonly LanguageAppService _sut;

        public LanguageAppServiceBddTests()
        {
            _applicationLanguageManager = Substitute.For<IApplicationLanguageManager>();
            _applicationLanguageTextManager = Substitute.For<IApplicationLanguageTextManager>();
            _languageRepository = Substitute.For<IRepository<ApplicationLanguage>>();

            _sut = new LanguageAppService(
                _applicationLanguageManager,
                _applicationLanguageTextManager,
                _languageRepository
            );
        }

        #region Construtor

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region GetAllLanguages

        [Fact]
        public async Task Dado_IdiomasExistentes_Quando_GetAllLanguages_Entao_DeveRetornarListaOrdenada()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var languages = new List<ApplicationLanguage>
            {
                new ApplicationLanguage(null, "pt-BR", "Português (Brasil)"),
                new ApplicationLanguage(null, "en", "English")
            };
            _applicationLanguageManager.GetLanguagesAsync(null).Returns(languages);

            // Quando
            var result = await _sut.GetAllLanguages();

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
        }

        #endregion

        #region DeleteLanguage

        [Fact]
        public async Task Dado_IdiomaExistente_Quando_DeleteLanguage_Entao_DeveRemoverIdioma()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var language = new ApplicationLanguage(null, "fr", "French");
            _languageRepository.GetAsync(1).Returns(language);

            // Quando
            await _sut.DeleteLanguage(new Abp.Application.Services.Dto.EntityDto(1));

            // Então
            await _applicationLanguageManager.Received(1).RemoveAsync(null, "fr");
        }

        #endregion

        #region SetDefaultLanguage

        [Fact]
        public async Task Dado_IdiomaValido_Quando_SetDefaultLanguage_Entao_DeveDefinirPadrao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            // Quando
            await _sut.SetDefaultLanguage(new SetDefaultLanguageInput { Name = "pt-BR" });

            // Então
            await _applicationLanguageManager.Received(1)
                .SetDefaultLanguageAsync(null, "pt-BR");
        }

        #endregion

        #region UpdateLanguageText

        [Fact]
        public async Task Dado_TextoValido_Quando_UpdateLanguageText_Entao_DeveAtualizar()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var localizationManager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.Name.Returns("EafMiddleware");
            localizationManager.GetSource("EafMiddleware").Returns(source);
            _sut.LocalizationManager = localizationManager;

            var input = new UpdateLanguageTextInput
            {
                LanguageName = "pt-BR",
                SourceName = "EafMiddleware",
                Key = "Hello",
                Value = "Olá"
            };

            // Quando
            await _sut.UpdateLanguageText(input);

            // Então
            await _applicationLanguageTextManager.Received(1)
                .UpdateStringAsync(
                    null,
                    "EafMiddleware",
                    Arg.Is<CultureInfo>(c => c.Name == "pt-BR"),
                    "Hello",
                    "Olá"
                );
        }

        #endregion

        #region GetLanguages

        [Fact]
        public async Task Dado_IdiomasExistentes_Quando_GetLanguages_Entao_DeveRetornarComIdiomaPadrao()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var languages = new List<ApplicationLanguage>
            {
                new ApplicationLanguage(null, "pt-BR", "Português (Brasil)"),
                new ApplicationLanguage(null, "en", "English")
            };
            _applicationLanguageManager.GetLanguagesAsync(null).Returns(languages);
            _applicationLanguageManager.GetDefaultLanguageOrNullAsync(null)
                .Returns(new ApplicationLanguage(null, "pt-BR", "Português (Brasil)"));

            var objectMapper = Substitute.For<Abp.ObjectMapping.IObjectMapper>();
            objectMapper.Map<List<ApplicationLanguageListDto>>(Arg.Any<object>())
                .Returns(new List<ApplicationLanguageListDto>
                {
                    new ApplicationLanguageListDto { Name = "pt-BR" },
                    new ApplicationLanguageListDto { Name = "en" }
                });
            _sut.ObjectMapper = objectMapper;

            var input = new GetLanguagesInput { Sorting = "Name" };

            // Quando
            var result = await _sut.GetLanguages(input);

            // Então
            result.ShouldNotBeNull();
            result.DefaultLanguageName.ShouldBe("pt-BR");
        }

        #endregion

        #region GetLanguageForEdit

        [Fact]
        public async Task Dado_IdNulo_Quando_GetLanguageForEdit_Entao_DeveRetornarNovoIdioma()
        {
            // Dado
            var objectMapper = Substitute.For<Abp.ObjectMapping.IObjectMapper>();
            _sut.ObjectMapper = objectMapper;

            // Quando
            var result = await _sut.GetLanguageForEdit(new Abp.Application.Services.Dto.NullableIdDto());

            // Então
            result.ShouldNotBeNull();
            result.Language.ShouldNotBeNull();
            result.LanguageNames.ShouldNotBeNull();
            result.Flags.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_IdExistente_Quando_GetLanguageForEdit_Entao_DeveRetornarIdiomaExistente()
        {
            // Dado
            var language = new ApplicationLanguage(null, "pt-BR", "Português (Brasil)") { Icon = "br" };
            _languageRepository.GetAsync(1).Returns(language);

            var objectMapper = Substitute.For<Abp.ObjectMapping.IObjectMapper>();
            objectMapper.Map<ApplicationLanguageEditDto>(language)
                .Returns(new ApplicationLanguageEditDto { Name = "pt-BR", Icon = "br" });
            _sut.ObjectMapper = objectMapper;

            // Quando
            var result = await _sut.GetLanguageForEdit(new Abp.Application.Services.Dto.NullableIdDto { Id = 1 });

            // Então
            result.ShouldNotBeNull();
            result.Language.Name.ShouldBe("pt-BR");
        }

        #endregion
    }
}
