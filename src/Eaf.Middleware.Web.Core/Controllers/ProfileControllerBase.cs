using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Eaf.Middleware.Authorization.Users.Profile;
using Eaf.Middleware.Net.MimeTypes;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Web.Helpers;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp;

namespace Eaf.Middleware.Web.Controllers
{
    /// <summary>
    /// Representa a classe ProfileControllerBase.
    /// </summary>
    public abstract class ProfileControllerBase : MiddlewareControllerBase, IApplicationService
    {
        private const int MaxProfilePictureSize = 5242880; //5MB
        private readonly IProfileAppService _profileAppService;
        private readonly ITempFileCacheManager _tempFileCacheManager;

        protected ProfileControllerBase(
            ITempFileCacheManager tempFileCacheManager,
            IProfileAppService profileAppService)
        {
            _tempFileCacheManager = tempFileCacheManager;
            _profileAppService = profileAppService;
        }

        [HttpPost]
        public JsonResult UploadProfilePicture()
        {
            try
            {
                var profilePictureFile = Request.Form.Files.First();

                //Check input
                if (profilePictureFile == null)
                {
                    throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
                }

                if (profilePictureFile.Length > MaxProfilePictureSize)
                {
                    throw new UserFriendlyException(L("ProfilePicture_Warn_SizeLimit", MaxProfilePictureSize));
                }

                byte[] fileBytes;
                using (var stream = profilePictureFile.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                }

                var formartImage = ImageFormatHelper.GetRawImageFormat(fileBytes);

                if (!formartImage.Name.IsIn("JPEG", "JPG", "BMP", "PNG", "GIF"))
                {
                    throw new AbpException(L("IncorrectImageFormat"));
                }


                var token = Guid.NewGuid().ToString();

                _tempFileCacheManager.SetFile(token, fileBytes);

                using (var image = Image.Load(fileBytes))
                {
                    return Json(new AjaxResponse(new
                    {
                        FileToken = token,
                        profilePictureFile.FileName,
                        FileType = profilePictureFile.ContentType,
                        image.Width,
                        image.Height
                    }));
                }
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [AllowAnonymous]
        public FileResult GetDefaultProfilePicture()
        {
            return GetDefaultProfilePictureInternal();
        }

        /// <summary>
        /// GetProfilePictureByUser.
        /// </summary>
        /// <param name="userId">Parâmetro userId.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<FileResult> GetProfilePictureByUser(long userId)
        {
            var output = await _profileAppService.GetProfilePictureByUser(userId);
            if (output.ProfilePicture.IsNullOrEmpty())
            {
                return GetDefaultProfilePictureInternal();
            }

            return File(Convert.FromBase64String(output.ProfilePicture), MimeTypeNames.ImageJpeg);
        }

        protected FileResult GetDefaultProfilePictureInternal()
        {
            return File(Path.Combine("Common", "Images", "default-profile-picture.png"), MimeTypeNames.ImagePng);
        }
    }
}