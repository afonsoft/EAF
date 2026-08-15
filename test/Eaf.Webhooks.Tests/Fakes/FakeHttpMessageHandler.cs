using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.Webhooks.Tests.Fakes
{
    /// <summary>
    /// Handler HTTP fake para capturar requisições e retornar respostas controladas nos testes.
    /// </summary>
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage LastRequest { get; private set; }

        public HttpResponseMessage Response { get; set; } = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(string.Empty) };

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(Response);
        }
    }
}
