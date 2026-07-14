#nullable disable
using Eaf.Middleware.Authorization.Users.Profile;
using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Abp;
using Abp.UI;
using Abp.Web.Models;
using NSubstitute;
using Shouldly;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Controllers
{
    public class ProfileControllerBaseBddTests
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IProfileAppService _profileAppService;

        public ProfileControllerBaseBddTests()
        {
            _tempFileCacheManager = Substitute.For<ITempFileCacheManager>();
            _profileAppService = Substitute.For<IProfileAppService>();
        }

        private sealed class TestableProfileController : ProfileControllerBase
        {
            public TestableProfileController(
                ITempFileCacheManager tempFileCacheManager,
                IProfileAppService profileAppService)
                : base(tempFileCacheManager, profileAppService)
            {
            }

            public new FileResult GetDefaultProfilePicture()
            {
                return base.GetDefaultProfilePicture();
            }
        }

        private static TestableProfileController CreateController(ITempFileCacheManager tempFileCacheManager, IProfileAppService profileAppService)
        {
            var httpContext = new DefaultHttpContext();
            return new TestableProfileController(tempFileCacheManager, profileAppService)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveInicializarCorretamente()
        {
            var sut = new TestableProfileController(_tempFileCacheManager, _profileAppService);
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveHerdarDeMiddlewareControllerBase()
        {
            var sut = new TestableProfileController(_tempFileCacheManager, _profileAppService);
            sut.ShouldBeAssignableTo<MiddlewareControllerBase>();
        }

        [Fact]
        public async Task Dado_UsuarioComFoto_Quando_GetProfilePictureByUser_Entao_DeveRetornarFileContentResult()
        {
            // Dado
            var pictureBytes = new byte[] { 0x01, 0x02, 0x03 };
            _profileAppService.GetProfilePictureByUser(1).Returns(new GetProfilePictureOutput(Convert.ToBase64String(pictureBytes)));
            var sut = CreateController(_tempFileCacheManager, _profileAppService);

            // Quando
            var result = await sut.GetProfilePictureByUser(1);

            // Então
            result.ShouldBeOfType<FileContentResult>();
            ((FileContentResult)result).FileContents.ShouldBe(pictureBytes);
        }

        [Fact]
        public async Task Dado_UsuarioSemFoto_Quando_GetProfilePictureByUser_Entao_DeveRetornarDefault()
        {
            // Dado
            _profileAppService.GetProfilePictureByUser(1).Returns(new GetProfilePictureOutput(string.Empty));
            var sut = CreateController(_tempFileCacheManager, _profileAppService);

            // Quando
            var result = await sut.GetProfilePictureByUser(1);

            // Então
            result.ShouldNotBeNull();
            result.ShouldBeAssignableTo<FileResult>();
        }

        [Fact]
        public async Task Dado_ModelStateInvalido_Quando_GetProfilePictureByUser_Entao_DeveLancarUserFriendlyException()
        {
            // Dado
            var sut = CreateController(_tempFileCacheManager, _profileAppService);
            sut.ModelState.AddModelError("userId", "Invalid");

            // Quando / Então
            var ex = await Should.ThrowAsync<UserFriendlyException>(async () => await sut.GetProfilePictureByUser(1));
            ex.Message.ShouldContain("InvalidRequest");
        }

        [Fact]
        public void Dado_Quando_GetDefaultProfilePicture_Entao_DeveRetornarFileResult()
        {
            // Dado
            var sut = CreateController(_tempFileCacheManager, _profileAppService);

            // Quando
            var result = sut.GetDefaultProfilePicture();

            // Então
            result.ShouldNotBeNull();
            result.ShouldBeAssignableTo<FileResult>();
        }

        [Fact]
        public void Dado_ArquivoPngValido_Quando_UploadProfilePicture_Entao_DeveRetornarJsonComDados()
        {
            // Dado
            var sut = CreateController(_tempFileCacheManager, _profileAppService);
            var file = CriarFormFilePng("profile.png");
            sut.Request.Form = new FormCollection(
                new System.Collections.Generic.Dictionary<string, StringValues>(),
                new FormFileCollection { file });

            // Quando
            var result = sut.UploadProfilePicture();

            // Então
            result.ShouldBeOfType<JsonResult>();
            var jsonResult = (JsonResult)result;
            jsonResult.Value.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ArquivoNulo_Quando_UploadProfilePicture_Entao_DeveLancarInvalidOperationException()
        {
            // Dado
            var sut = CreateController(_tempFileCacheManager, _profileAppService);
            sut.Request.Form = new FormCollection(
                new System.Collections.Generic.Dictionary<string, StringValues>(),
                new FormFileCollection());

            // Quando & Então
            Should.Throw<System.InvalidOperationException>(() => sut.UploadProfilePicture());
        }

        [Fact]
        public void Dado_ArquivoMaiorQueLimite_Quando_UploadProfilePicture_Entao_DeveRetornarErro()
        {
            // Dado
            var sut = CreateController(_tempFileCacheManager, _profileAppService);
            var file = new FormFile(
                new MemoryStream(new byte[5242881]),
                0,
                5242881,
                "file",
                "large.png")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };
            sut.Request.Form = new FormCollection(
                new System.Collections.Generic.Dictionary<string, StringValues>(),
                new FormFileCollection { file });

            // Quando
            var result = sut.UploadProfilePicture();

            // Então
            result.ShouldBeOfType<JsonResult>();
            var jsonResult = (JsonResult)result;
            var error = (ErrorInfo)jsonResult.Value.GetType().GetProperty("Error")?.GetValue(jsonResult.Value);
            error.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ArquivoNuloNaLista_Quando_UploadProfilePicture_Entao_DeveRetornarErroJson()
        {
            var sut = CreateController(_tempFileCacheManager, _profileAppService);
            sut.Request.Form = new FormCollection(
                new System.Collections.Generic.Dictionary<string, StringValues>(),
                new FormFileCollection { null! });

            var result = sut.UploadProfilePicture();

            result.ShouldBeOfType<JsonResult>();
            var jsonResult = (JsonResult)result;
            var error = jsonResult.Value.GetType().GetProperty("Error")?.GetValue(jsonResult.Value);
            error.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ArquivoFormatoInvalido_Quando_UploadProfilePicture_Entao_DeveLancarAbpException()
        {
            var sut = CreateController(_tempFileCacheManager, _profileAppService);
            var file = CriarFormFileTiff("image.tiff");
            sut.Request.Form = new FormCollection(
                new System.Collections.Generic.Dictionary<string, StringValues>(),
                new FormFileCollection { file });

            Should.Throw<AbpException>(() => sut.UploadProfilePicture());
        }

        private static IFormFile CriarFormFileTiff(string fileName)
        {
            using var image = new Image<Rgba32>(1, 1);
            var stream = new MemoryStream();
            image.SaveAsTiff(stream);
            stream.Position = 0;

            return new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/tiff"
            };
        }

        private static IFormFile CriarFormFilePng(string fileName)
        {
            using var image = new Image<Rgba32>(1, 1);
            var stream = new MemoryStream();
            image.SaveAsPng(stream);
            stream.Position = 0;

            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };

            return formFile;
        }
    }
}
