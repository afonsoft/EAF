#nullable disable
using Abp.Domain.Uow;
using Eaf.Middleware.Chat;
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
    public class ChatControllerBaseBddTests
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IChatMessageManager _chatMessageManager;

        public ChatControllerBaseBddTests()
        {
            _binaryObjectManager = Substitute.For<IBinaryObjectManager>();
            _chatMessageManager = Substitute.For<IChatMessageManager>();
        }

        private ChatController CreateController()
        {
            var httpContext = new DefaultHttpContext();
            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            activeUow.SaveChangesAsync().Returns(Task.CompletedTask);

            var uowManager = Substitute.For<IUnitOfWorkManager>();
            uowManager.Current.Returns(activeUow);

            return new ChatController(_binaryObjectManager, _chatMessageManager)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext },
                UnitOfWorkManager = uowManager
            };
        }

        private static IFormFile CriarFormFileTexto(string fileName, string content)
        {
            var stream = new MemoryStream();
            using var writer = new StreamWriter(stream, leaveOpen: true);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            return formFile;
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarChatController_Entao_DeveInicializarCorretamente()
        {
            var sut = new ChatController(_binaryObjectManager, _chatMessageManager);
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_BinaryObjectManager_Quando_CriarChatController_Entao_DeveHerdarDeMiddlewareControllerBase()
        {
            var sut = new ChatController(_binaryObjectManager, _chatMessageManager);
            sut.ShouldBeAssignableTo<MiddlewareControllerBase>();
        }

        [Fact]
        public async Task Dado_ArquivoValido_Quando_UploadFile_Entao_DeveSalvarObjetoERetornarJson()
        {
            // Dado
            var sut = CreateController();
            var file = CriarFormFileTexto("document.txt", "conteudo P25");
            sut.Request.Form = new FormCollection(
                new System.Collections.Generic.Dictionary<string, StringValues>(),
                new FormFileCollection { file });

            var binaryObject = new BinaryObject(null, new byte[] { }, "text/plain", "document.txt");
            _binaryObjectManager.SaveAsync(Arg.Any<BinaryObject>()).Returns(Task.CompletedTask).AndDoes(x => binaryObject = x.Arg<BinaryObject>());

            // Quando
            var result = await sut.UploadFile();

            // Então
            result.ShouldBeOfType<JsonResult>();
            await _binaryObjectManager.Received(1).SaveAsync(Arg.Any<BinaryObject>());
        }

        [Fact]
        public async Task Dado_ArquivoNulo_Quando_UploadFile_Entao_DeveRetornarErroJson()
        {
            // Dado
            var sut = CreateController();
            sut.Request.Form = new FormCollection(
                new System.Collections.Generic.Dictionary<string, StringValues>(),
                new FormFileCollection());

            // Quando
            var result = await sut.UploadFile();

            // Então
            result.ShouldBeOfType<JsonResult>();
        }
    }
}
