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

        private static async Task<(DefaultHttpContext context, PublicErrorContract error)> InvocarComExcecao(Exception exception)
        {
            var middleware = new EafPublicErrorMiddleware(
                _ => throw exception,
                NullLogger<EafPublicErrorMiddleware>.Instance);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.Invoke(context);

            context.Response.Body.Position = 0;
            var error = await JsonSerializer.DeserializeAsync<PublicErrorContract>(context.Response.Body, JsonOptions);
            return (context, error);
        }

        [Fact]
        public async Task Dado_AbpValidationException_Quando_InvocarMiddleware_Entao_DeveRetornar400()
        {
            var (context, error) = await InvocarComExcecao(new Abp.Runtime.Validation.AbpValidationException("Validation failed"));

            context.Response.StatusCode.ShouldBe(400);
            error.ShouldNotBeNull();
            error.Code.ShouldBe(EafErrorCodes.ValidationFailed);
            error.Retryable.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_AbpAuthorizationException_Quando_InvocarMiddleware_Entao_DeveRetornar403()
        {
            var (context, error) = await InvocarComExcecao(new Abp.Authorization.AbpAuthorizationException("Forbidden"));

            context.Response.StatusCode.ShouldBe(403);
            error.ShouldNotBeNull();
            error.Code.ShouldBe(EafErrorCodes.NotAuthorized);
            error.Retryable.ShouldBeFalse();
        }

        [Fact]
        public async Task Dado_EntityNotFoundException_Quando_InvocarMiddleware_Entao_DeveRetornar404()
        {
            var (context, error) = await InvocarComExcecao(new Abp.Domain.Entities.EntityNotFoundException("Not found"));

            context.Response.StatusCode.ShouldBe(404);
            error.ShouldNotBeNull();
            error.Code.ShouldBe(EafErrorCodes.ValidationFailed);
        }

        [Fact]
        public async Task Dado_TimeoutException_Quando_InvocarMiddleware_Entao_DeveRetornar500Retryable()
        {
            var (context, error) = await InvocarComExcecao(new TimeoutException("Timeout"));

            context.Response.StatusCode.ShouldBe(500);
            error.ShouldNotBeNull();
            error.Code.ShouldBe(EafErrorCodes.TemporarilyUnavailable);
            error.Retryable.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_ExcecaoGenerica_Quando_InvocarMiddleware_Entao_DeveRetornar500Generico()
        {
            var (context, error) = await InvocarComExcecao(new Exception("Unexpected"));

            context.Response.StatusCode.ShouldBe(500);
            error.ShouldNotBeNull();
            error.Code.ShouldBe(EafErrorCodes.TemporarilyUnavailable);
            error.Retryable.ShouldBeTrue();
            error.CorrelationId.ShouldBe(context.TraceIdentifier);
        }

        [Fact]
        public async Task Dado_RequestAbortado_Quando_InvocarMiddleware_Entao_DeveRelancarExcecao()
        {
            // Dado
            var middleware = new EafPublicErrorMiddleware(
                _ => throw new InvalidOperationException("aborted"),
                NullLogger<EafPublicErrorMiddleware>.Instance);

            var context = new DefaultHttpContext();
            var cts = new System.Threading.CancellationTokenSource();
            cts.Cancel();
            context.RequestAborted = cts.Token;

            // Quando / Então
            await Should.ThrowAsync<InvalidOperationException>(() => middleware.Invoke(context));
        }
    }
}
