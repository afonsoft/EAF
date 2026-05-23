using Abp.Auditing;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Extensions;
using Abp.Localization;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Abp.Web.Models.AbpUserConfiguration;
using Abp.Zero.Configuration;
using Eaf.Middleware.Authorization.Users.Dto;
using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Security;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Timing;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authorization.Users.Profile
{
    [AbpAuthorize]
    public class ProfileAppService : MiddlewareAppServiceBase, IProfileAppService
    {
        private const int MaxProfilPictureBytes = 5242880; //5MB
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ITypedCache<string, AbpUserConfigurationDto> _eafUserConfigurationCache;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly ITimeZoneService _timeZoneService;

        /// <summary>
        /// ProfileAppService.
        /// </summary>
        /// <param name="binaryObjectManager">Parâmetro binaryObjectManager.</param>
        /// <param name="timezoneService">Parâmetro timezoneService.</param>
        /// <param name="tempFileCacheManager">Parâmetro tempFileCacheManager.</param>
        /// <param name="cacheManager">Parâmetro cacheManager.</param>
        /// <returns>Resultado da operação.</returns>
        public ProfileAppService(
            IBinaryObjectManager binaryObjectManager,
            ITimeZoneService timezoneService,
            ITempFileCacheManager tempFileCacheManager,
            ICacheManager cacheManager
        )
        {
            _binaryObjectManager = binaryObjectManager;
            _timeZoneService = timezoneService;
            _tempFileCacheManager = tempFileCacheManager;
            _eafUserConfigurationCache = cacheManager.GetCache<string, AbpUserConfigurationDto>("EafUserConfiguration");
        }

        /// <summary>
        /// ChangeLanguage.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );

            await _eafUserConfigurationCache.ClearAsync();
        }

        /// <summary>
        /// ChangePassword.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task ChangePassword(ChangePasswordInput input)
        {
            await UserManager.InitializeOptionsAsync(AbpSession.TenantId);

            var user = await GetCurrentUserAsync();
            CheckErrors(await UserManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword));
        }

        [DisableAuditing]
        public async Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEdit()
        {
            var user = await GetCurrentUserAsync();
            var userProfileEditDto = ObjectMapper.Map<CurrentUserProfileEditDto>(user);

            if (Clock.SupportsMultipleTimezone)
            {
                userProfileEditDto.Timezone = await SettingManager.GetSettingValueAsync(TimingSettingNames.TimeZone);

                var defaultTimeZoneId =
                    await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.User, AbpSession.TenantId);
                if (userProfileEditDto.Timezone == defaultTimeZoneId)
                {
                    userProfileEditDto.Timezone = string.Empty;
                }
            }

            return userProfileEditDto;
        }

        [AbpAllowAnonymous]
        public async Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting()
        {
            var passwordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit =
                    await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement
                        .PasswordComplexity.RequireDigit),
                RequireLowercase = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames
                    .UserManagement.PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames
                    .UserManagement.PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames
                    .UserManagement.PasswordComplexity.RequireUppercase),
                RequiredLength = await SettingManager.GetSettingValueAsync<int>(AbpZeroSettingNames.UserManagement
                    .PasswordComplexity.RequiredLength)
            };

            return new GetPasswordComplexitySettingOutput
            {
                Setting = passwordComplexitySetting
            };
        }

        [DisableAuditing]
        public async Task<GetProfilePictureOutput> GetProfilePicture()
        {
            try
            {
                var user = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());
                if (user.ProfilePictureId == null)
                {
                    return new GetProfilePictureOutput(string.Empty);
                }

                return await GetProfilePictureById(user.ProfilePictureId.Value);
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "Error on GetProfilePicture {0} : {1}", AbpSession.UserId, ex.Message);
                return new GetProfilePictureOutput(string.Empty);
            }
        }

        [DisableAuditing]
        [HttpGet]
        public async Task<GetProfilePictureOutput> GetProfilePictureByUser(long userId)
        {
            try
            {
                var user = await UserManager.GetUserByIdAsync(userId);
                if (user.ProfilePictureId == null)
                {
                    return new GetProfilePictureOutput(string.Empty);
                }

                return await GetProfilePictureById(user.ProfilePictureId.Value);
            }
            catch (Exception ex)
            {
                Logger.DebugFormat(ex, "GetProfilePictureByUser {0}:{1}", userId, ex.Message);
                return new GetProfilePictureOutput(string.Empty);
            }
        }

        [DisableAuditing]
        [HttpGet]
        public async Task<GetProfilePictureOutput> GetFriendProfilePicture(long userId, long tenantId)
        {
            try
            {
                var user = await UserManager.GetUserByIdAsync(userId);
                if (user.ProfilePictureId == null)
                {
                    return new GetProfilePictureOutput(string.Empty);
                }

                return await GetProfilePictureById(user.ProfilePictureId.Value);
            }
            catch (Exception ex)
            {
                Logger.DebugFormat(ex, "GetFriendProfilePicture {0}|(1):{2}", userId, tenantId, ex.Message);
                return new GetProfilePictureOutput(string.Empty);
            }
        }

        /// <summary>
        /// GetProfilePictureById.
        /// </summary>
        /// <param name="profilePictureId">Parâmetro profilePictureId.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<GetProfilePictureOutput> GetProfilePictureById(Guid profilePictureId)
        {
            return await GetProfilePictureByIdInternal(profilePictureId);
        }

        /// <summary>
        /// UpdateCurrentUserProfile.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task UpdateCurrentUserProfile(CurrentUserProfileEditDto input)
        {
            var user = await GetCurrentUserAsync();

            ObjectMapper.Map(input, user);
            CheckErrors(await UserManager.UpdateAsync(user));

            if (Clock.SupportsMultipleTimezone)
            {
                if (input.Timezone.IsNullOrEmpty())
                {
                    var defaultValue =
                        await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.User, AbpSession.TenantId);
                    await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(),
                        TimingSettingNames.TimeZone, defaultValue);
                }
                else
                {
                    await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(),
                        TimingSettingNames.TimeZone, input.Timezone);
                }
            }

            await _eafUserConfigurationCache.ClearAsync();
        }

        /// <summary>
        /// UpdateProfilePicture.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task UpdateProfilePicture(UpdateProfilePictureInput input)
        {
            byte[] byteArray;

            var imageBytes = _tempFileCacheManager.GetFile(input.FileToken);

            if (imageBytes == null)
            {
                throw new UserFriendlyException("There is no such image file with the token: " + input.FileToken);
            }

            using (var image = Image.Load(imageBytes))
            {
                var clone = image.Clone(ctx => ctx.Crop(new Rectangle(input.X, input.Y, input.Width > 0 ? input.Width : image.Width, input.Height > 0 ? input.Height : image.Height)));
                using (var stream = new MemoryStream())
                {
                    await clone.SaveAsBmpAsync(stream);
                    byteArray = stream.ToArray();
                }
            }

            if (byteArray.Length > MaxProfilPictureBytes)
            {
                throw new UserFriendlyException(L("ResizedProfilePicture_Warn_SizeLimit",
                    MiddlewareAppConsts.ResizedMaxProfilPictureBytesUserFriendlyValue));
            }

            var user = await UserManager.GetUserByIdAsync(AbpSession.GetUserId());

            var contentType = ".bmp";
            var fileName = $"{Guid.NewGuid()}.bmp";

            var storedFile = new BinaryObject(null, byteArray, contentType, fileName);
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                if (user.ProfilePictureId.HasValue)
                {
                    await _binaryObjectManager.DeleteAsync(user.ProfilePictureId.Value);
                }

                await _binaryObjectManager.SaveAsync(storedFile);
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            user.ProfilePictureId = storedFile.Id;
            await CurrentUnitOfWork.SaveChangesAsync();

            await _eafUserConfigurationCache.ClearAsync();
        }

        [HttpGet]
        [AbpAllowAnonymous]
        public async Task<GetProfilePictureOutput> GetFriendProfilePictureById(Guid profilePictureId, long userId,
            long tenantId)
        {
            var user = await UserManager.GetUserByIdAsync(userId);
            if (user.ProfilePictureId != profilePictureId)
            {
                return new GetProfilePictureOutput(string.Empty);
            }

            return await GetProfilePictureById(profilePictureId);
        }

        private async Task<GetProfilePictureOutput> GetProfilePictureByIdInternal(Guid profilePictureId)
        {
            var bytes = await GetProfilePictureByIdOrNull(profilePictureId);
            if (bytes == null)
            {
                return new GetProfilePictureOutput(string.Empty);
            }

            return new GetProfilePictureOutput(Convert.ToBase64String(bytes));
        }

        private async Task<byte[]> GetProfilePictureByIdOrNull(Guid profilePictureId)
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var file = await _binaryObjectManager.GetOrNullAsync(profilePictureId);
                if (file == null)
                {
                    return null;
                }

                return file.Bytes;
            }
        }
    }
}