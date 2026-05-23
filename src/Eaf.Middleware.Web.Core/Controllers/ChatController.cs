using Abp;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Runtime.Security;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Storage;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Controllers
{
    [AbpAllowAnonymous]
    public class ChatController : ChatControllerBase
    {
        /// <summary>
        /// ChatController.
        /// </summary>
        /// <param name="binaryObjectManager">Parâmetro binaryObjectManager.</param>
        /// <param name="chatMessageManager">Parâmetro chatMessageManager.</param>
        /// <returns>Resultado da operação.</returns>
        public ChatController(IBinaryObjectManager binaryObjectManager, IChatMessageManager chatMessageManager) :
            base(binaryObjectManager, chatMessageManager)
        {
        }

        [DisableAuditing]
        [HttpGet]
        public async Task<FileContentResult> GetUploadedObject(Guid fileId, string fileName, string contentType, string enc_auth_token)
        {
            var jwtToken = SimpleStringCipher.Instance.Decrypt(enc_auth_token, MiddlewareCoreConsts.DefaultPassPhrase);
            if (string.IsNullOrEmpty(jwtToken))
                throw new AbpException(L("NotFound"));

            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var fileObject = await BinaryObjectManager.GetOrNullAsync(fileId);
                if (fileObject == null)
                {
                    throw new AbpException(L("NotFound"));
                }

                return File(fileObject.Bytes, contentType, fileName);
            }
        }
    }
}