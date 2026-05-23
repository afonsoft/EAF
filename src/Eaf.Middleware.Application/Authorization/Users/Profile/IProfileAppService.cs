using Abp.Application.Services;
using Eaf.Middleware.Authorization.Users.Dto;
using Eaf.Middleware.Authorization.Users.Profile.Dto;
using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authorization.Users.Profile
{
    /// <summary>
    /// Representa a interface IProfileAppService.
    /// </summary>
    public interface IProfileAppService : IApplicationService
    {
        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task ChangePassword(ChangePasswordInput input);

        Task<CurrentUserProfileEditDto> GetCurrentUserProfileForEdit();

        Task<GetPasswordComplexitySettingOutput> GetPasswordComplexitySetting();

        Task<GetProfilePictureOutput> GetProfilePicture();

        Task<GetProfilePictureOutput> GetProfilePictureByUser(long userId);

        Task<GetProfilePictureOutput> GetFriendProfilePicture(long userId, long tenantId);

        Task<GetProfilePictureOutput> GetProfilePictureById(Guid profilePictureId);

        Task UpdateCurrentUserProfile(CurrentUserProfileEditDto input);

        Task UpdateProfilePicture(UpdateProfilePictureInput input);
    }
}