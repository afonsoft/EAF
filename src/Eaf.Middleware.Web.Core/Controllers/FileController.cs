using Abp.Auditing;
using Abp.Authorization;
using Eaf.Middleware.Dto;
using Eaf.Middleware.Storage;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Abp.Extensions;
using Abp.IO.Extensions;
using Abp;

namespace Eaf.Middleware.Web.Controllers
{
    /// <summary>
    /// Representa a classe FileController.
    /// </summary>
    public class FileController : MiddlewareControllerBase
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;

        /// <summary>
        /// FileController.
        /// </summary>
        /// <param name="tempFileCacheManager">Parâmetro tempFileCacheManager.</param>
        /// <param name="binaryObjectManager">Parâmetro binaryObjectManager.</param>
        /// <returns>Resultado da operação.</returns>
        public FileController(
            ITempFileCacheManager tempFileCacheManager,
            IBinaryObjectManager binaryObjectManager
        )
        {
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
        }

        [Route("[controller]/[action]")]
        [HttpGet]
        [DisableAuditing]
        public async Task<ActionResult> DownloadTempFile(FileDto file)
        {
            var fileBytes = _tempFileCacheManager.GetFile(file.FileToken);
            if (fileBytes == null)
            {
                BinaryObject fileDb = null;
                using (CurrentUnitOfWork.SetTenantId(null))
                {
                    fileDb = await _binaryObjectManager.GetOrNullAsync(new Guid(file.FileToken));
                    if (fileDb == null && !file.FileName.IsNullOrEmpty())
                        fileDb = await _binaryObjectManager.GetOrNullAsync(file.FileName);
                    if (fileDb == null)
                        return NotFound(L("RequestedFileDoesNotExists"));

                    var download = File(fileDb.Bytes, fileDb.FileType, fileDb.FileName);
                    await _binaryObjectManager.DeleteAsync(fileDb.Id);
                    return download;
                }
            }
            return File(fileBytes, file.FileType, file.FileName);
        }

        [Route("[controller]/[action]")]
        [HttpGet]
        [DisableAuditing]
        public async Task<ActionResult> DownloadBinaryFile(Guid id, string contentType = null, string fileName = null)
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var fileObject = await _binaryObjectManager.GetOrNullAsync(id);
                if (fileObject == null)
                {
                    if (!fileName.IsNullOrEmpty())
                        fileObject = await _binaryObjectManager.GetOrNullAsync(fileName);

                    if (fileObject == null)
                        return NotFound(L("RequestedFileDoesNotExists"));
                }
                return File(fileObject.Bytes, contentType ?? fileObject.FileType, fileName ?? fileObject.FileName);
            }
        }

        [Route("[controller]/[action]")]
        [HttpPost]
        [AbpAuthorize]
        [DisableAuditing]
        public JsonResult UploadTempFile()
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

                var id = SequentialGuidGenerator.Instance.Create().ToString();
                _tempFileCacheManager.SetFile(id, fileBytes);

                return Json(new AjaxResponse(new
                {
                    id = id,
                    name = file.FileName,
                    contentType = file.ContentType
                }));
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [Route("[controller]/[action]")]
        [HttpPost]
        [AbpAuthorize]
        [DisableAuditing]
        public async Task<JsonResult> UploadBinaryFile()
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
                    fileObject.Id = await _binaryObjectManager.SaveAndGetIdAsync(fileObject);
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