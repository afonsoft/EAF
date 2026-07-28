using Abp.UI;
using Eaf.Middleware.Contracts;
using Eaf.Middleware.Web.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Web.Tests.Middleware
{
    public class EafPublicErrorMiddlewareBddTests
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        [Fact]
        public async Task Dado_UserFriendlyException_Quando_InvocarMiddleware_Entao_DeveRetornar400ComMensagem()
        {
            // Dado
            var middleware = new EafPublicErrorMiddleware(
                _ => throw new UserFriendlyException("Invalid credentials"),
                NullLogger<EafPublicErrorMiddleware>.Instance);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Quando
            await middleware.Invoke(context);

            // Então
            context.Response.StatusCode.ShouldBe(400);
            context.Response.ContentType.ShouldStartWith("application/json");

            context.Response.Body.Position = 0;
            var error = await JsonSerializer.DeserializeAsync<PublicErrorContract>(context.Response.Body, JsonOptions);
            error.ShouldNotBeNull();
            error.Code.ShouldBe(EafErrorCodes.ValidationFailed);
            error.Message.ShouldBe("Invalid credentials");
            error.Retryable.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_ArgumentException_Quando_InvocarMiddleware_Entao_DeveRetornar400()
        {
            // Dado
            var middleware = new EafPublicErrorMiddleware(
                _ => throw new ArgumentException("Invalid argument"),
                NullLogger<EafPublicErrorMiddleware>.Instance);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Quando
            await middleware.Invoke(context);

            // Então
            context.Response.StatusCode.ShouldBe(400);
            context.Response.Body.Position = 0;
            var error = await JsonSerializer.DeserializeAsync<PublicErrorContract>(context.Response.Body, JsonOptions);
            error.ShouldNotBeNull();
            error.Code.ShouldBe(EafErrorCodes.ValidationFailed);
        }

        [Fact]
        public async Task Dado_InvalidOperationException_Quando_InvocarMiddleware_Entao_DeveRetornar500ERetryable()
        {
            // Dado
            var middleware = new EafPublicErrorMiddleware(
                _ => throw new InvalidOperationException("Service unavailable"),
                NullLogger<EafPublicErrorMiddleware>.Instance);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Quando
            await middleware.Invoke(context);

            // Então
            context.Response.StatusCode.ShouldBe(500);
            context.Response.Body.Position = 0;
            var error = await JsonSerializer.DeserializeAsync<PublicErrorContract>(context.Response.Body, JsonOptions);
            error.ShouldNotBeNull();
            error.Code.ShouldBe(EafErrorCodes.TemporarilyUnavailable);
            error.Retryable.ShouldBeTrue();
        }
    }
}
