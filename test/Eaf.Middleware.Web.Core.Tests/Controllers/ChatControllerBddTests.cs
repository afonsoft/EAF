#nullable disable
using Abp.Domain.Uow;
using Abp.Runtime.Security;
using Eaf.Middleware;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Controllers
{
    public class ChatControllerBddTests
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IChatMessageManager _chatMessageManager;

        public ChatControllerBddTests()
        {
            _binaryObjectManager = Substitute.For<IBinaryObjectManager>();
            _chatMessageManager = Substitute.For<IChatMessageManager>();
        }

        private ChatController CreateController()
        {
            var httpContext = new DefaultHttpContext();
            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());

            var uowManager = Substitute.For<IUnitOfWorkManager>();
            uowManager.Current.Returns(activeUow);

            return new ChatController(_binaryObjectManager, _chatMessageManager)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext },
                UnitOfWorkManager = uowManager
            };
        }

        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var binaryObjectManager = Substitute.For<IBinaryObjectManager>();
            var chatMessageManager = Substitute.For<IChatMessageManager>();
            var sut = new ChatController(binaryObjectManager, chatMessageManager);
            sut.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_ArquivoExistente_Quando_GetUploadedObject_Entao_DeveRetornarFileContentResult()
        {
            // Dado
            var fileId = Guid.NewGuid();
            var fileName = "doc.txt";
            var contentType = "text/plain";
            var fileBytes = new byte[] { 0x01, 0x02, 0x03 };
            var binaryObject = new BinaryObject(null, fileBytes, contentType, fileName) { Id = fileId };

            var token = SimpleStringCipher.Instance.Encrypt("token-valido", MiddlewareCoreConsts.DefaultPassPhrase);
            _binaryObjectManager.GetOrNullAsync(fileId).Returns(binaryObject);

            var sut = CreateController();

            // Quando
            var result = await sut.GetUploadedObject(fileId, fileName, contentType, token);

            // Então
            result.ShouldBeOfType<FileContentResult>();
            ((FileContentResult)result).FileContents.ShouldBe(fileBytes);
        }

        [Fact]
        public async Task Dado_ArquivoInexistente_Quando_GetUploadedObject_Entao_DeveLancarAbpException()
        {
            // Dado
            var fileId = Guid.NewGuid();
            var token = SimpleStringCipher.Instance.Encrypt("token-valido", MiddlewareCoreConsts.DefaultPassPhrase);
            _binaryObjectManager.GetOrNullAsync(fileId).Returns((BinaryObject)null);

            var sut = CreateController();

            // Quando & Então
            await Should.ThrowAsync<Abp.AbpException>(async () => await sut.GetUploadedObject(fileId, "doc.txt", "text/plain", token));
        }

        [Fact]
        public async Task Dado_TokenVazio_Quando_GetUploadedObject_Entao_DeveLancarAbpException()
        {
            // Dado
            var fileId = Guid.NewGuid();
            var sut = CreateController();

            // Quando & Então
            await Should.ThrowAsync<Abp.AbpException>(async () => await sut.GetUploadedObject(fileId, "doc.txt", "text/plain", string.Empty));
        }
    }
}
