using Abp.Application.Services;
using Abp.Authorization;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Storage;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.IO.Extensions;

namespace Eaf.Middleware.Web.Controllers
{
    /// <summary>
    /// Representa a classe ChatControllerBase.
    /// </summary>
    public abstract class ChatControllerBase : MiddlewareControllerBase, IApplicationService
    {
        protected readonly IBinaryObjectManager BinaryObjectManager;
        protected readonly IChatMessageManager ChatMessageManager;

        protected ChatControllerBase(IBinaryObjectManager binaryObjectManager, IChatMessageManager chatMessageManager)
        {
            BinaryObjectManager = binaryObjectManager;
            ChatMessageManager = chatMessageManager;
        }

        [HttpPost]
        [AbpAuthorize]
        public async Task<JsonResult> UploadFile()
        {
            try
            {
                var file = Request.Form.Files[0];

                //Check input
                if (file == null)
                {
                    throw new UserFriendlyException(L("File_Empty_Error"));
                }

                if (file.Length > 20000000) //20MB
                {
                    throw new UserFriendlyException(L("File_SizeLimit_Error"));
                }

                byte[] fileBytes;
                using (var stream = file.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                }

                var fileObject = new BinaryObject(null, fileBytes, file.ContentType, file.FileName);
                using (CurrentUnitOfWork.SetTenantId(null))
                {
                    await BinaryObjectManager.SaveAsync(fileObject);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }

                return Json(new AjaxResponse(new
                {
                    id = fileObject.Id,
                    name = file.FileName,
                    contentType = file.ContentType
                }));
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
    }
}