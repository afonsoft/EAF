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

        #region CreateOrUpdateLanguage

        [Fact]
        public async Task Dado_NovoIdioma_Quando_CreateOrUpdateLanguage_Entao_DeveAdicionarIdioma()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            _applicationLanguageManager.GetLanguagesAsync(null).Returns(new List<ApplicationLanguage>());

            var input = new CreateOrUpdateLanguageInput
            {
                Language = new ApplicationLanguageEditDto
                {
                    Name = "fr",
                    Icon = "fr",
                    IsEnabled = true
                }
            };

            // Quando
            await _sut.CreateOrUpdateLanguage(input);

            // Então
            await _applicationLanguageManager.Received(1).AddAsync(Arg.Is<ApplicationLanguage>(l => l.Name == "fr"));
        }

        [Fact]
        public async Task Dado_IdiomaExistente_Quando_CreateOrUpdateLanguage_Entao_DeveAtualizarIdioma()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var existingLanguage = new ApplicationLanguage(null, "pt-BR", "Português (Brasil)") { Id = 1 };
            _applicationLanguageManager.GetLanguagesAsync(null).Returns(new List<ApplicationLanguage> { existingLanguage });
            _languageRepository.GetAsync(1).Returns(existingLanguage);

            var input = new CreateOrUpdateLanguageInput
            {
                Language = new ApplicationLanguageEditDto
                {
                    Id = 1,
                    Name = "pt-BR",
                    Icon = "br",
                    IsEnabled = true
                }
            };

            // Quando
            await _sut.CreateOrUpdateLanguage(input);

            // Então
            await _applicationLanguageManager.Received(1).UpdateAsync(null, existingLanguage);
        }

        #endregion

        #region GetLanguageTexts

        [Fact]
        public async Task Dado_TextosLocalizados_Quando_GetLanguageTexts_Entao_DeveRetornarListaPaginada()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var localizationManager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.Name.Returns("EafMiddleware");
            source.GetAllStrings().Returns(new List<LocalizedString>
            {
                new LocalizedString("Hello", "Hello", CultureInfo.InvariantCulture)
            });
            localizationManager.GetSource("EafMiddleware").Returns(source);
            _sut.LocalizationManager = localizationManager;

            _applicationLanguageTextManager.GetStringOrNull(
                Arg.Any<int?>(),
                Arg.Any<string>(),
                Arg.Any<CultureInfo>(),
                Arg.Any<string>(),
                Arg.Any<bool>()).Returns("Olá");

            var input = new GetLanguageTextsInput
            {
                BaseLanguageName = "pt-BR",
                TargetLanguageName = "en-US",
                SourceName = "EafMiddleware",
                Sorting = "Key",
                MaxResultCount = 10,
                SkipCount = 0,
                TargetValueFilter = "ALL"
            };

            // Quando
            var result = await _sut.GetLanguageTexts(input);

            // Então
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(1);
            result.Items.Count.ShouldBe(1);
            result.Items[0].Key.ShouldBe("Hello");
        }

        [Fact]
        public async Task Dado_BaseLanguageNaoInformado_Quando_GetLanguageTexts_Entao_DeveUsarIdiomaPadraoComoBase()
        {
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var localizationManager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.Name.Returns("EafMiddleware");
            source.GetAllStrings().Returns(new List<LocalizedString>
            {
                new LocalizedString("Hello", "Hello", CultureInfo.InvariantCulture)
            });
            localizationManager.GetSource("EafMiddleware").Returns(source);
            _sut.LocalizationManager = localizationManager;

            _applicationLanguageManager.GetDefaultLanguageOrNullAsync(null).Returns((ApplicationLanguage)null);
            _applicationLanguageManager.GetLanguagesAsync(null).Returns(new List<ApplicationLanguage>
            {
                new ApplicationLanguage(null, "en", "English")
            });

            _applicationLanguageTextManager.GetStringOrNull(
                Arg.Any<int?>(),
                Arg.Any<string>(),
                Arg.Any<CultureInfo>(),
                Arg.Any<string>(),
                Arg.Any<bool>()).Returns("Olá");

            var input = new GetLanguageTextsInput
            {
                BaseLanguageName = null,
                TargetLanguageName = "pt-BR",
                SourceName = "EafMiddleware",
                MaxResultCount = 10,
                SkipCount = 0,
                TargetValueFilter = "ALL"
            };

            var result = await _sut.GetLanguageTexts(input);

            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(1);
        }

        [Fact]
        public async Task Dado_TextosVaziosEComFiltro_Quando_GetLanguageTexts_Entao_DeveRetornarTextosFiltrados()
        {
            // Dado
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);
            _sut.AbpSession = abpSession;

            var localizationManager = Substitute.For<ILocalizationManager>();
            var source = Substitute.For<ILocalizationSource>();
            source.Name.Returns("EafMiddleware");
            source.GetAllStrings().Returns(new List<LocalizedString>
            {
                new LocalizedString("Hello", "Hello", CultureInfo.InvariantCulture),
                new LocalizedString("World", "World", CultureInfo.InvariantCulture)
            });
            localizationManager.GetSource("EafMiddleware").Returns(source);
            _sut.LocalizationManager = localizationManager;

            _applicationLanguageTextManager.GetStringOrNull(
                Arg.Any<int?>(),
                Arg.Any<string>(),
                Arg.Any<CultureInfo>(),
                Arg.Any<string>(),
                Arg.Any<bool>()).Returns(ci =>
            {
                var culture = ci.ArgAt<CultureInfo>(2);
                var name = ci.ArgAt<string>(3);
                return culture.Name == "en-US" ? null : name;
            });

            var input = new GetLanguageTextsInput
            {
                BaseLanguageName = "pt-BR",
                TargetLanguageName = "en-US",
                SourceName = "EafMiddleware",
                MaxResultCount = 10,
                SkipCount = 0,
                TargetValueFilter = "EMPTY",
                FilterText = "Hello"
            };

            // Quando
            var result = await _sut.GetLanguageTexts(input);

            // Então
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(1);
            result.Items[0].Key.ShouldBe("Hello");
            result.Items[0].BaseValue.ShouldBe("Hello");
            result.Items[0].TargetValue.ShouldBeNull();
        }

        #endregion
    }
}
