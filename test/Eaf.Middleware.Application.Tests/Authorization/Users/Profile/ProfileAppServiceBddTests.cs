using Abp;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Uow;
using Abp.Localization;
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

        [Fact]
        public async Task Dado_ErroDeIdentidade_Quando_ChangePassword_Entao_DeveLancarExcecao()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns(user);
            userManager.ChangePasswordAsync(user, "oldPass", "newPass")
                .Returns(IdentityResult.Failed(new IdentityError { Description = "Current password is incorrect" }));

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns((int?)null);

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;
            _sut.LocalizationManager = Substitute.For<ILocalizationManager>();

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(() =>
                _sut.ChangePassword(new ChangePasswordInput { CurrentPassword = "oldPass", NewPassword = "newPass" }));
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

        #region GetProfilePictureById

        [Fact]
        public async Task Dado_ImagemExistente_Quando_GetProfilePictureById_Entao_DeveRetornarBase64()
        {
            // Dado
            var profilePictureId = Guid.NewGuid();
            var bytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
            var binaryObject = new BinaryObject(null, bytes, ".jpg", "picture.jpg");

            _binaryObjectManager.GetOrNullAsync(profilePictureId).Returns(binaryObject);

            var currentUow = Substitute.For<IActiveUnitOfWork>();
            currentUow.SetTenantId(default(int?)).ReturnsForAnyArgs(Substitute.For<IDisposable>());

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(currentUow);
            _sut.UnitOfWorkManager = unitOfWorkManager;

            // Quando
            var result = await _sut.GetProfilePictureById(profilePictureId);

            // Então
            result.ShouldNotBeNull();
            result.ProfilePicture.ShouldBe(Convert.ToBase64String(bytes));
        }

        #endregion

        #region GetProfilePictureByUser

        [Fact]
        public async Task Dado_UsuarioComFoto_Quando_GetProfilePictureByUser_Entao_DeveRetornarBase64()
        {
            // Dado
            var profilePictureId = Guid.NewGuid();
            var bytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
            var user = new User { Id = 1, UserName = "admin", ProfilePictureId = profilePictureId };
            var binaryObject = new BinaryObject(null, bytes, ".jpg", "picture.jpg");

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);

            _binaryObjectManager.GetOrNullAsync(profilePictureId).Returns(binaryObject);

            var currentUow = Substitute.For<IActiveUnitOfWork>();
            currentUow.SetTenantId(default(int?)).ReturnsForAnyArgs(Substitute.For<IDisposable>());

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(currentUow);
            _sut.UnitOfWorkManager = unitOfWorkManager;

            _sut.UserManager = userManager;

            // Quando
            var result = await _sut.GetProfilePictureByUser(1);

            // Então
            result.ShouldNotBeNull();
            result.ProfilePicture.ShouldBe(Convert.ToBase64String(bytes));
        }

        #endregion

        #region GetFriendProfilePictureById

        [Fact]
        public async Task Dado_AmigoComFotoCorreta_Quando_GetFriendProfilePictureById_Entao_DeveRetornarBase64()
        {
            // Dado
            var profilePictureId = Guid.NewGuid();
            var bytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
            var user = new User { Id = 2, UserName = "friend", ProfilePictureId = profilePictureId };
            var binaryObject = new BinaryObject(null, bytes, ".jpg", "picture.jpg");

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(2).Returns(user);

            _binaryObjectManager.GetOrNullAsync(profilePictureId).Returns(binaryObject);

            var currentUow = Substitute.For<IActiveUnitOfWork>();
            currentUow.SetTenantId(default(int?)).ReturnsForAnyArgs(Substitute.For<IDisposable>());

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(currentUow);
            _sut.UnitOfWorkManager = unitOfWorkManager;

            _sut.UserManager = userManager;

            // Quando
            var result = await _sut.GetFriendProfilePictureById(profilePictureId, 2, 1);

            // Então
            result.ShouldNotBeNull();
            result.ProfilePicture.ShouldBe(Convert.ToBase64String(bytes));
        }

        [Fact]
        public async Task Dado_AmigoComFotoDiferente_Quando_GetFriendProfilePictureById_Entao_DeveRetornarVazio()
        {
            // Dado
            var profilePictureId = Guid.NewGuid();
            var user = new User { Id = 2, UserName = "friend", ProfilePictureId = Guid.NewGuid() };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(2).Returns(user);

            _sut.UserManager = userManager;

            // Quando
            var result = await _sut.GetFriendProfilePictureById(profilePictureId, 2, 1);

            // Então
            result.ShouldNotBeNull();
            result.ProfilePicture.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task Dado_AmigoSemFoto_Quando_GetFriendProfilePictureById_Entao_DeveRetornarVazio()
        {
            var profilePictureId = Guid.NewGuid();
            var user = new User { Id = 2, UserName = "friend", ProfilePictureId = null };

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(2).Returns(user);

            _sut.UserManager = userManager;

            var result = await _sut.GetFriendProfilePictureById(profilePictureId, 2, 1);

            result.ShouldNotBeNull();
            result.ProfilePicture.ShouldBe(string.Empty);
        }

        #endregion

        #region GetFriendProfilePicture

        [Fact]
        public async Task Dado_AmigoComFoto_Quando_GetFriendProfilePicture_Entao_DeveRetornarBase64()
        {
            // Dado
            var profilePictureId = Guid.NewGuid();
            var bytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
            var user = new User { Id = 2, UserName = "friend", ProfilePictureId = profilePictureId };
            var binaryObject = new BinaryObject(null, bytes, ".jpg", "picture.jpg");

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(2).Returns(user);

            _binaryObjectManager.GetOrNullAsync(profilePictureId).Returns(binaryObject);

            var currentUow = Substitute.For<IActiveUnitOfWork>();
            currentUow.SetTenantId(default(int?)).ReturnsForAnyArgs(Substitute.For<IDisposable>());

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(currentUow);
            _sut.UnitOfWorkManager = unitOfWorkManager;

            _sut.UserManager = userManager;

            // Quando
            var result = await _sut.GetFriendProfilePicture(2, 1);

            // Então
            result.ShouldNotBeNull();
            result.ProfilePicture.ShouldBe(Convert.ToBase64String(bytes));
        }

        #endregion

        #region GetCurrentUserProfileForEdit / UpdateCurrentUserProfile with Timezone

        [Fact]
        public async Task Dado_UsuarioLogadoSemSuporteATimezone_Quando_GetCurrentUserProfileForEdit_Entao_DeveRetornarTimezoneVazio()
        {
            var user = new User { Id = 1, UserName = "admin", Name = "Admin", Surname = "User" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns(user);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns((int?)null);

            var originalProvider = Abp.Timing.Clock.Provider;
            var clockProvider = Substitute.For<IClockProvider>();
            clockProvider.SupportsMultipleTimezone.Returns(false);
            Abp.Timing.Clock.Provider = clockProvider;

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;
            _sut.ObjectMapper = CreateObjectMapper();

            try
            {
                var result = await _sut.GetCurrentUserProfileForEdit();

                result.ShouldNotBeNull();
                result.Timezone.ShouldBeNullOrEmpty();
            }
            finally
            {
                Abp.Timing.Clock.Provider = originalProvider;
            }
        }

        [Fact]
        public async Task Dado_UsuarioLogadoComTimezone_Quando_GetCurrentUserProfileForEdit_Entao_DeveRetornarTimezone()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin", Name = "Admin", Surname = "User" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns(user);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns((int?)null);

            var originalProvider = Abp.Timing.Clock.Provider;
            var clockProvider = Substitute.For<IClockProvider>();
            clockProvider.SupportsMultipleTimezone.Returns(true);
            Abp.Timing.Clock.Provider = clockProvider;

            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueAsync(Arg.Is<string>(s => s.Contains("TimeZone"))).Returns("America/Sao_Paulo");

            _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.User, Arg.Any<int?>()).Returns("UTC");

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;
            _sut.ObjectMapper = CreateObjectMapper();
            _sut.SettingManager = settingManager;

            try
            {
                // Quando
                var result = await _sut.GetCurrentUserProfileForEdit();

                // Então
                result.ShouldNotBeNull();
                result.Timezone.ShouldBe("America/Sao_Paulo");
            }
            finally
            {
                Abp.Timing.Clock.Provider = originalProvider;
            }
        }

        [Fact]
        public async Task Dado_PerfilValidoComTimezone_Quando_UpdateCurrentUserProfile_Entao_DeveAtualizarTimezone()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin", Name = "Admin", Surname = "User" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns(user);
            userManager.UpdateAsync(user).Returns(IdentityResult.Success);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns((int?)null);

            var originalProvider = Abp.Timing.Clock.Provider;
            var clockProvider = Substitute.For<IClockProvider>();
            clockProvider.SupportsMultipleTimezone.Returns(true);
            Abp.Timing.Clock.Provider = clockProvider;

            var settingManager = Substitute.For<ISettingManager>();
            settingManager.ChangeSettingForUserAsync(Arg.Any<UserIdentifier>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.CompletedTask);

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;
            _sut.ObjectMapper = CreateObjectMapper();
            _sut.SettingManager = settingManager;

            try
            {
                // Quando
                await _sut.UpdateCurrentUserProfile(new CurrentUserProfileEditDto { Name = "Admin", Surname = "User", Timezone = "America/Sao_Paulo" });

                // Então
                await settingManager.Received(1).ChangeSettingForUserAsync(
                    Arg.Any<UserIdentifier>(),
                    TimingSettingNames.TimeZone,
                    "America/Sao_Paulo");
            }
            finally
            {
                Abp.Timing.Clock.Provider = originalProvider;
            }
        }

        #endregion

        #region UpdateProfilePicture

        [Fact]
        public async Task Dado_ImagemValida_Quando_UpdateProfilePicture_Entao_DeveSalvarNovaFoto()
        {
            // Dado
            var fileToken = "token123";
            var imageBytes = new byte[] { 66, 77, 58, 0, 0, 0, 0, 0, 0, 0, 54, 0, 0, 0, 40, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 24, 0, 0, 0, 0, 0, 4, 0, 0, 0, 196, 14, 0, 0, 196, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            _tempFileCacheManager.GetFile(fileToken).Returns(imageBytes);

            var user = new User { Id = 1, UserName = "admin" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);

            _sut.UserManager = userManager;

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            activeUow.SaveChangesAsync().Returns(Task.CompletedTask);

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);
            _sut.UnitOfWorkManager = unitOfWorkManager;

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            _sut.AbpSession = abpSession;

            _binaryObjectManager.SaveAsync(Arg.Any<BinaryObject>()).Returns(Task.CompletedTask);

            var input = new UpdateProfilePictureInput
            {
                FileToken = fileToken,
                X = 0,
                Y = 0,
                Width = 0,
                Height = 0
            };

            // Quando
            await _sut.UpdateProfilePicture(input);

            // Então
            await _binaryObjectManager.Received(1).SaveAsync(Arg.Any<BinaryObject>());
            user.ProfilePictureId.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ImagemValidaComCropEFotoExistente_Quando_UpdateProfilePicture_Entao_DeveDeletarFotoAntigaESalvarNova()
        {
            // Dado
            var fileToken = "token123";
            var imageBytes = new byte[] { 66, 77, 58, 0, 0, 0, 0, 0, 0, 0, 54, 0, 0, 0, 40, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 24, 0, 0, 0, 0, 0, 4, 0, 0, 0, 196, 14, 0, 0, 196, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            _tempFileCacheManager.GetFile(fileToken).Returns(imageBytes);

            var existingPictureId = Guid.NewGuid();
            var user = new User { Id = 1, UserName = "admin", ProfilePictureId = existingPictureId };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);

            _sut.UserManager = userManager;

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            activeUow.SaveChangesAsync().Returns(Task.CompletedTask);

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);
            _sut.UnitOfWorkManager = unitOfWorkManager;

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            _sut.AbpSession = abpSession;

            _binaryObjectManager.DeleteAsync(existingPictureId).Returns(Task.CompletedTask);
            _binaryObjectManager.SaveAsync(Arg.Any<BinaryObject>()).Returns(Task.CompletedTask);

            var input = new UpdateProfilePictureInput
            {
                FileToken = fileToken,
                X = 0,
                Y = 0,
                Width = 1,
                Height = 1
            };

            // Quando
            await _sut.UpdateProfilePicture(input);

            // Então
            await _binaryObjectManager.Received(1).DeleteAsync(existingPictureId);
            await _binaryObjectManager.Received(1).SaveAsync(Arg.Any<BinaryObject>());
            user.ProfilePictureId.ShouldNotBe(existingPictureId);
        }

        [Fact]
        public async Task Dado_ImagemMaiorQueLimite_Quando_UpdateProfilePicture_Entao_DeveLancarExcecaoDeTamanho()
        {
            // Dado
            var fileToken = "token-limite";
            var imageBytes = CreateBmp(1400, 1400);

            _tempFileCacheManager.GetFile(fileToken).Returns(imageBytes);

            var input = new UpdateProfilePictureInput
            {
                FileToken = fileToken,
                X = 0,
                Y = 0,
                Width = 0,
                Height = 0
            };

            // Quando & Então
            await Should.ThrowAsync<UserFriendlyException>(() => _sut.UpdateProfilePicture(input));
        }

        #endregion

        [Fact]
        public async Task Dado_UsuarioSemFoto_Quando_GetProfilePictureById_Entao_DeveRetornarVazio()
        {
            var profilePictureId = Guid.NewGuid();
            _binaryObjectManager.GetOrNullAsync(profilePictureId).Returns((BinaryObject)null);

            var currentUow = Substitute.For<IActiveUnitOfWork>();
            currentUow.SetTenantId(Arg.Any<int?>()).ReturnsForAnyArgs(Substitute.For<IDisposable>());

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(currentUow);
            _sut.UnitOfWorkManager = unitOfWorkManager;

            var result = await _sut.GetProfilePictureById(profilePictureId);

            result.ShouldNotBeNull();
            result.ProfilePicture.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task Dado_UsuarioComFoto_Quando_GetProfilePicture_Entao_DeveRetornarBase64()
        {
            var profilePictureId = Guid.NewGuid();
            var bytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
            var user = new User { Id = 1, UserName = "admin", ProfilePictureId = profilePictureId };
            var binaryObject = new BinaryObject(null, bytes, ".jpg", "picture.jpg");

            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);

            _binaryObjectManager.GetOrNullAsync(profilePictureId).Returns(binaryObject);

            var currentUow = Substitute.For<IActiveUnitOfWork>();
            currentUow.SetTenantId(Arg.Any<int?>()).ReturnsForAnyArgs(Substitute.For<IDisposable>());

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(currentUow);
            _sut.UnitOfWorkManager = unitOfWorkManager;
            _sut.UserManager = userManager;

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.GetUserId().Returns(1L);
            _sut.AbpSession = abpSession;

            var result = await _sut.GetProfilePicture();

            result.ShouldNotBeNull();
            result.ProfilePicture.ShouldBe(Convert.ToBase64String(bytes));
        }

        [Fact]
        public async Task Dado_UsuarioInexistente_Quando_GetProfilePicture_Entao_DeveRetornarVazio()
        {
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(Task.FromException<User>(new UserFriendlyException("User not found")));

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.GetUserId().Returns(1L);
            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;

            var result = await _sut.GetProfilePicture();

            result.ShouldNotBeNull();
            result.ProfilePicture.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task Dado_UsuarioComFotoNula_Quando_GetProfilePictureByUser_Entao_DeveRetornarVazio()
        {
            var user = new User { Id = 1, UserName = "admin", ProfilePictureId = null };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(1).Returns(user);

            _sut.UserManager = userManager;

            var result = await _sut.GetProfilePictureByUser(1);

            result.ShouldNotBeNull();
            result.ProfilePicture.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task Dado_UsuarioInexistente_Quando_GetProfilePictureByUser_Entao_DeveRetornarVazio()
        {
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(99).Returns(Task.FromException<User>(new UserFriendlyException("User not found")));

            _sut.UserManager = userManager;

            var result = await _sut.GetProfilePictureByUser(99);

            result.ShouldNotBeNull();
            result.ProfilePicture.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task Dado_UsuarioComFotoNula_Quando_GetFriendProfilePicture_Entao_DeveRetornarVazio()
        {
            var user = new User { Id = 2, UserName = "friend", ProfilePictureId = null };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(2).Returns(user);

            _sut.UserManager = userManager;

            var result = await _sut.GetFriendProfilePicture(2, 1);

            result.ShouldNotBeNull();
            result.ProfilePicture.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task Dado_UsuarioInexistente_Quando_GetFriendProfilePicture_Entao_DeveRetornarVazio()
        {
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.GetUserByIdAsync(99).Returns(Task.FromException<User>(new UserFriendlyException("User not found")));

            _sut.UserManager = userManager;

            var result = await _sut.GetFriendProfilePicture(99, 1);

            result.ShouldNotBeNull();
            result.ProfilePicture.ShouldBe(string.Empty);
        }

        [Fact]
        public async Task Dado_UsuarioLogadoComTimezoneIgualPadrao_Quando_GetCurrentUserProfileForEdit_Entao_DeveRetornarTimezoneVazio()
        {
            var user = new User { Id = 1, UserName = "admin", Name = "Admin", Surname = "User" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns(user);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns((int?)null);

            var originalProvider = Abp.Timing.Clock.Provider;
            var clockProvider = Substitute.For<IClockProvider>();
            clockProvider.SupportsMultipleTimezone.Returns(true);
            Abp.Timing.Clock.Provider = clockProvider;

            var settingManager = Substitute.For<ISettingManager>();
            settingManager.GetSettingValueAsync(Arg.Is<string>(s => s.Contains("TimeZone"))).Returns("UTC");

            _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.User, Arg.Any<int?>()).Returns("UTC");

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;
            _sut.ObjectMapper = CreateObjectMapper();
            _sut.SettingManager = settingManager;

            try
            {
                var result = await _sut.GetCurrentUserProfileForEdit();

                result.ShouldNotBeNull();
                result.Timezone.ShouldBe(string.Empty);
            }
            finally
            {
                Abp.Timing.Clock.Provider = originalProvider;
            }
        }

        [Fact]
        public async Task Dado_UsuarioLogadoSemTimezone_Quando_UpdateCurrentUserProfile_Entao_DeveAtualizarParaPadrao()
        {
            var user = new User { Id = 1, UserName = "admin", Name = "Admin", Surname = "User" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByIdAsync("1").Returns(user);
            userManager.UpdateAsync(user).Returns(IdentityResult.Success);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns((int?)null);

            var originalProvider = Abp.Timing.Clock.Provider;
            var clockProvider = Substitute.For<IClockProvider>();
            clockProvider.SupportsMultipleTimezone.Returns(true);
            Abp.Timing.Clock.Provider = clockProvider;

            var settingManager = Substitute.For<ISettingManager>();
            settingManager.ChangeSettingForUserAsync(Arg.Any<UserIdentifier>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.CompletedTask);

            _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.User, Arg.Any<int?>()).Returns("UTC");

            _sut.AbpSession = abpSession;
            _sut.UserManager = userManager;
            _sut.ObjectMapper = CreateObjectMapper();
            _sut.SettingManager = settingManager;

            try
            {
                await _sut.UpdateCurrentUserProfile(new CurrentUserProfileEditDto { Name = "Admin", Surname = "User", Timezone = string.Empty });

                await settingManager.Received(1).ChangeSettingForUserAsync(
                    Arg.Any<UserIdentifier>(),
                    TimingSettingNames.TimeZone,
                    "UTC");
            }
            finally
            {
                Abp.Timing.Clock.Provider = originalProvider;
            }
        }

        private IObjectMapper CreateObjectMapper()
        {
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<CurrentUserProfileEditDto>(Arg.Any<object>()).Returns(new CurrentUserProfileEditDto { Name = "Admin" });
            objectMapper.Map<CurrentUserProfileEditDto, User>(Arg.Any<CurrentUserProfileEditDto>(), Arg.Any<User>()).Returns(user => user.Arg<User>());
            return objectMapper;
        }

        private static byte[] CreateBmp(int width, int height)
        {
            var rowSize = ((width * 3 + 3) / 4) * 4;
            var pixelDataSize = rowSize * height;
            var fileSize = 54 + pixelDataSize;
            var bytes = new byte[fileSize];

            // BMP header
            bytes[0] = 0x42; bytes[1] = 0x4D;
            BitConverter.GetBytes(fileSize).CopyTo(bytes, 2);
            BitConverter.GetBytes(0).CopyTo(bytes, 6);
            BitConverter.GetBytes(54).CopyTo(bytes, 10);

            // DIB header
            BitConverter.GetBytes(40).CopyTo(bytes, 14);
            BitConverter.GetBytes(width).CopyTo(bytes, 18);
            BitConverter.GetBytes(height).CopyTo(bytes, 22);
            bytes[26] = 1; bytes[27] = 0;
            bytes[28] = 24; bytes[29] = 0;
            BitConverter.GetBytes(0).CopyTo(bytes, 30);
            BitConverter.GetBytes(pixelDataSize).CopyTo(bytes, 34);
            BitConverter.GetBytes(0).CopyTo(bytes, 38);
            BitConverter.GetBytes(0).CopyTo(bytes, 42);
            BitConverter.GetBytes(0).CopyTo(bytes, 46);
            BitConverter.GetBytes(0).CopyTo(bytes, 50);

            return bytes;
        }
    }
}
