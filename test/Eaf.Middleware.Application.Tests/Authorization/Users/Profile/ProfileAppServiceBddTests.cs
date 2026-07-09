using Abp;
using Abp.Authorization;
using Abp.Configuration;
using Abp.ObjectMapping;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Authorization.Users.Profile;
using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Timing;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
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

        #region ChangePassword

        [Fact]
        public async Task Dado_SenhaCorreta_Quando_ChangePassword_Entao_DeveAlterarSenha()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns(user);
            userManager.ChangePasswordAsync(user, "oldPass", "newPass").Returns(IdentityResult.Success);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns((int?)null);

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;

            // Quando
            await _sut.ChangePassword(new ChangePasswordInput { CurrentPassword = "oldPass", NewPassword = "newPass" });

            // Então
            await userManager.Received(1).ChangePasswordAsync(user, "oldPass", "newPass");
        }

        #endregion

        #region GetCurrentUserProfileForEdit

        [Fact]
        public async Task Dado_UsuarioLogado_Quando_GetCurrentUserProfileForEdit_Entao_DeveRetornarPerfil()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin", Name = "Admin", Surname = "User" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns(user);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns((int?)null);

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;
            _sut.ObjectMapper = CreateObjectMapper();

            // Quando
            var result = await _sut.GetCurrentUserProfileForEdit();

            // Então
            result.ShouldNotBeNull();
            result.Name.ShouldBe("Admin");
        }

        #endregion

        #region GetPasswordComplexitySetting

        [Fact]
        public async Task Dado_ConfiguracoesPadrao_Quando_GetPasswordComplexitySetting_Entao_DeveRetornarValores()
        {
            // Dado
            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueAsync(Arg.Any<string>()).Returns(ci =>
            {
                var name = ci.Arg<string>();
                return name != null && name.EndsWith("RequiredLength") ? "8" : "True";
            });

            _sut.SettingManager = settingManager;

            // Quando
            var result = await _sut.GetPasswordComplexitySetting();

            // Então
            result.ShouldNotBeNull();
            result.Setting.ShouldNotBeNull();
            result.Setting.RequireDigit.ShouldBeTrue();
            result.Setting.RequiredLength.ShouldBe(8);
        }

        #endregion

        #region GetProfilePicture

        [Fact]
        public async Task Dado_UsuarioSemFoto_Quando_GetProfilePicture_Entao_DeveRetornarVazio()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin", ProfilePictureId = null };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.GetUserId().Returns(1L);

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;

            // Quando
            var result = await _sut.GetProfilePicture();

            // Então
            result.ShouldNotBeNull();
            result.ProfilePicture.ShouldBe(string.Empty);
        }

        #endregion

        #region UpdateCurrentUserProfile

        [Fact]
        public async Task Dado_PerfilValido_Quando_UpdateCurrentUserProfile_Entao_DeveAtualizarUsuario()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin", Name = "Admin", Surname = "User" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns(user);
            userManager.UpdateAsync(user).Returns(IdentityResult.Success);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns((int?)null);

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;
            _sut.ObjectMapper = CreateObjectMapper();

            // Quando
            await _sut.UpdateCurrentUserProfile(new CurrentUserProfileEditDto { Name = "Admin", Surname = "User" });

            // Então
            await userManager.Received(1).UpdateAsync(user);
        }

        #endregion

        private IObjectMapper CreateObjectMapper()
        {
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<CurrentUserProfileEditDto>(Arg.Any<object>()).Returns(new CurrentUserProfileEditDto { Name = "Admin" });
            objectMapper.Map<CurrentUserProfileEditDto, User>(Arg.Any<CurrentUserProfileEditDto>(), Arg.Any<User>()).Returns(user => user.Arg<User>());
            return objectMapper;
        }
    }
}
