using Abp.Domain.Uow;
using Eaf.Middleware.Dto;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Shouldly;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Controllers
{
    /// <summary>
    /// Testes BDD para FileController seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class FileControllerBddTests
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly FileController _sut;

        public FileControllerBddTests()
        {
            _tempFileCacheManager = Substitute.For<ITempFileCacheManager>();
            _binaryObjectManager = Substitute.For<IBinaryObjectManager>();
            _sut = new FileController(_tempFileCacheManager, _binaryObjectManager);

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var activeUnitOfWork = Substitute.For<IActiveUnitOfWork>();
            activeUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns((IDisposable)null);
            unitOfWorkManager.Current.Returns(activeUnitOfWork);
            _sut.UnitOfWorkManager = unitOfWorkManager;
        }

        #region Instanciacao

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            _sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_TempFileCacheManager_Quando_CriarInstancia_Entao_DeveAceitarDependencia()
        {
            var tempManager = Substitute.For<ITempFileCacheManager>();
            var binaryManager = Substitute.For<IBinaryObjectManager>();
            var controller = new FileController(tempManager, binaryManager);
            controller.ShouldNotBeNull();
        }

        #endregion

        #region DownloadTempFile

        [Fact]
        public async Task Dado_ArquivoNoCache_Quando_DownloadTempFile_Entao_DeveRetornarFileContentResult()
        {
            var token = Guid.NewGuid().ToString();
            var bytes = new byte[] { 1, 2, 3 };
            _tempFileCacheManager.GetFile(token).Returns(bytes);

            var resultado = await _sut.DownloadTempFile(new FileDto { FileToken = token, FileType = "text/plain", FileName = "test.txt" });

            var fileResult = resultado.ShouldBeOfType<FileContentResult>();
            fileResult.ContentType.ShouldBe("text/plain");
            fileResult.FileDownloadName.ShouldBe("test.txt");
            fileResult.FileContents.ShouldBe(bytes);
        }

        [Fact]
        public async Task Dado_ArquivoNoBinaryManager_Quando_DownloadTempFile_Entao_DeveRetornarFileContentResultEDeletar()
        {
            var token = Guid.NewGuid().ToString();
            var bytes = new byte[] { 4, 5, 6 };
            var binaryObject = new BinaryObject(null, bytes, "application/pdf", "doc.pdf");
            _tempFileCacheManager.GetFile(token).Returns((byte[])null);
            _binaryObjectManager.GetOrNullAsync(new Guid(token)).Returns(Task.FromResult(binaryObject));

            var resultado = await _sut.DownloadTempFile(new FileDto { FileToken = token, FileType = "application/pdf", FileName = "doc.pdf" });

            var fileResult = resultado.ShouldBeOfType<FileContentResult>();
            fileResult.ContentType.ShouldBe(binaryObject.FileType);
            fileResult.FileDownloadName.ShouldBe(binaryObject.FileName);
            await _binaryObjectManager.Received(1).DeleteAsync(binaryObject.Id);
        }

        [Fact]
        public async Task Dado_ArquivoNaoEncontrado_Quando_DownloadTempFile_Entao_DeveRetornarNotFound()
        {
            var token = Guid.NewGuid().ToString();
            _tempFileCacheManager.GetFile(token).Returns((byte[])null);
            _binaryObjectManager.GetOrNullAsync(Arg.Any<Guid>()).Returns(Task.FromResult<BinaryObject>(null));
            _binaryObjectManager.GetOrNullAsync(Arg.Any<string>()).Returns(Task.FromResult<BinaryObject>(null));

            var resultado = await _sut.DownloadTempFile(new FileDto { FileToken = token, FileType = "text/plain", FileName = "missing.txt" });

            resultado.ShouldBeOfType<NotFoundObjectResult>();
        }

        #endregion

        #region DownloadBinaryFile

        [Fact]
        public async Task Dado_BinaryFileExistente_Quando_DownloadBinaryFile_Entao_DeveRetornarFileContentResult()
        {
            var id = Guid.NewGuid();
            var bytes = new byte[] { 7, 8, 9 };
            var binaryObject = new BinaryObject(null, bytes, "image/png", "image.png");
            _binaryObjectManager.GetOrNullAsync(id).Returns(Task.FromResult(binaryObject));

            var resultado = await _sut.DownloadBinaryFile(id, "image/png", "image.png");

            var fileResult = resultado.ShouldBeOfType<FileContentResult>();
            fileResult.ContentType.ShouldBe("image/png");
            fileResult.FileDownloadName.ShouldBe("image.png");
            fileResult.FileContents.ShouldBe(bytes);
        }

        [Fact]
        public async Task Dado_BinaryFilePorNome_Quando_DownloadBinaryFile_Entao_DeveRetornarFileContentResult()
        {
            var id = Guid.NewGuid();
            var bytes = new byte[] { 10, 11, 12 };
            var binaryObject = new BinaryObject(null, bytes, "image/jpeg", "foto.jpg");
            _binaryObjectManager.GetOrNullAsync(id).Returns(Task.FromResult<BinaryObject>(null));
            _binaryObjectManager.GetOrNullAsync("foto.jpg").Returns(Task.FromResult(binaryObject));

            var resultado = await _sut.DownloadBinaryFile(id, null, "foto.jpg");

            var fileResult = resultado.ShouldBeOfType<FileContentResult>();
            fileResult.ContentType.ShouldBe("image/jpeg");
            fileResult.FileDownloadName.ShouldBe("foto.jpg");
        }

        #endregion

        #region UploadTempFile

        [Fact]
        public void Dado_ArquivoValido_Quando_UploadTempFile_Entao_DeveRetornarAjaxResponseComId()
        {
            var file = CriarFormFile("arquivo.txt", "text/plain", new byte[] { 1, 2, 3 });
            ConfigurarRequestComArquivo(file);

            var resultado = _sut.UploadTempFile();

            var json = resultado.ShouldBeOfType<JsonResult>();
            json.Value.ShouldNotBeNull();
            _tempFileCacheManager.Received(1).SetFile(Arg.Any<string>(), Arg.Any<byte[]>());
        }

        [Fact]
        public void Dado_ArquivoMuitoGrande_Quando_UploadTempFile_Entao_DeveRetornarErro()
        {
            var file = CriarFormFile("grande.bin", "application/octet-stream", new byte[20000001]);
            ConfigurarRequestComArquivo(file);

            var resultado = _sut.UploadTempFile();

            var json = resultado.ShouldBeOfType<JsonResult>();
            var ajaxResponse = json.Value.ShouldBeOfType<Abp.Web.Models.AjaxResponse>();
            ajaxResponse.Success.ShouldBeFalse();
        }

        #endregion

        #region UploadBinaryFile

        [Fact]
        public async Task Dado_ArquivoValido_Quando_UploadBinaryFile_Entao_DeveRetornarAjaxResponseComId()
        {
            var file = CriarFormFile("binario.bin", "application/octet-stream", new byte[] { 1, 2, 3 });
            ConfigurarRequestComArquivo(file);
            var id = Guid.NewGuid();
            _binaryObjectManager.SaveAndGetIdAsync(Arg.Any<BinaryObject>()).Returns(Task.FromResult(id));

            var resultado = await _sut.UploadBinaryFile();

            var json = resultado.ShouldBeOfType<JsonResult>();
            json.Value.ShouldNotBeNull();
        }

        #endregion

        #region Helpers

        private static IFormFile CriarFormFile(string fileName, string contentType, byte[] content)
        {
            var stream = new MemoryStream(content);
            var formFile = new FormFile(stream, 0, content.Length, "file", fileName);
            formFile.Headers = new HeaderDictionary();
            formFile.ContentType = contentType;
            return formFile;
        }

        private void ConfigurarRequestComArquivo(IFormFile file)
        {
            var httpContext = new DefaultHttpContext();
            var formFiles = new FormFileCollection { file };
            var form = new FormCollection(new System.Collections.Generic.Dictionary<string, StringValues>(), formFiles);
            httpContext.Request.Form = form;
            _sut.ControllerContext = new ControllerContext { HttpContext = httpContext };
        }

        #endregion
    }
}
