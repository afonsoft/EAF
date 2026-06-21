using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.UI;
using Eaf.Middleware.Authorization.Users.Profile;
using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Timing;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Profile
{
    /// <summary>
    /// Testes BDD para ProfileAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ProfileAppServiceBddTests
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ITimeZoneService _timeZoneService;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly ICacheManager _cacheManager;
        private readonly ProfileAppService _sut;

        public ProfileAppServiceBddTests()
        {
            _binaryObjectManager = Substitute.For<IBinaryObjectManager>();
            _timeZoneService = Substitute.For<ITimeZoneService>();
            _tempFileCacheManager = Substitute.For<ITempFileCacheManager>();
            _cacheManager = Substitute.For<ICacheManager>();
            _cacheManager.GetCache(Arg.Any<string>()).Returns(Substitute.For<ICache>());

            _sut = new ProfileAppService(
                _binaryObjectManager,
                _timeZoneService,
                _tempFileCacheManager,
                _cacheManager
            );
        }

        #region Construtor

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        #endregion

        #region ChangeLanguage

        [Fact]
        public async Task Dado_IdiomaValido_Quando_ChangeLanguage_Entao_DeveAlterarConfiguracao()
        {
            // Dado
            var userIdentifier = new Abp.UserIdentifier(1, 42);
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);
            abpSession.UserId.Returns(42L);
            _sut.AbpSession = abpSession;

            var settingManager = Substitute.For<Abp.Configuration.ISettingManager>();
            _sut.SettingManager = settingManager;

            // Quando
            await _sut.ChangeLanguage(new Eaf.Middleware.Authorization.Users.Dto.ChangeUserLanguageDto { LanguageName = "pt-BR" });

            // Então
            await settingManager.Received(1).ChangeSettingForUserAsync(
                Arg.Any<Abp.UserIdentifier>(),
                Abp.Localization.LocalizationSettingNames.DefaultLanguage,
                "pt-BR"
            );
        }

        #endregion

        #region UpdateProfilePicture

        [Fact]
        public async Task Dado_TokenInvalido_Quando_UpdateProfilePicture_Entao_DeveLancarExcecao()
        {
            // Dado
            _tempFileCacheManager.GetFile("token-invalido").Returns((byte[])null);

            var input = new UpdateProfilePictureInput
            {
                FileToken = "token-invalido",
                X = 0,
                Y = 0,
                Width = 100,
                Height = 100
            };

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() => _sut.UpdateProfilePicture(input));
        }

        #endregion
    }
}
