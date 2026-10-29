using Eaf.AspNetCore.Configuration;
using Shouldly;
using System.Diagnostics;
using Xunit;

namespace Eaf.OpenTelemetry.Tests.AspNetCore.Configuration
{
    /// <summary>
    /// Testes BDD para PiiRedactionProcessor seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class PiiRedactionProcessorBddTests
    {
        private static Activity CriarActivity()
        {
            using var listener = new ActivityListener
            {
                ShouldListenTo = _ => true,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            };
            ActivitySource.AddActivityListener(listener);

            using var source = new ActivitySource("Eaf.Tests");
            return source.StartActivity("operacao-teste", ActivityKind.Server);
        }

        [Fact]
        public void Dado_TagComChaveSensivel_Quando_Finalizar_Entao_DeveRedigir()
        {
            // Dado
            var processor = new PiiRedactionProcessor();
            using var activity = CriarActivity();
            activity.ShouldNotBeNull();
            activity.SetTag("Authorization", "Bearer token");
            activity.SetTag("http.custom", "ok");

            // Quando
            processor.OnEnd(activity);

            // Então
            activity.GetTagItem("Authorization").ShouldBe("[REDACTED]");
            activity.GetTagItem("http.custom").ShouldBe("ok");
        }

        [Theory]
        [InlineData("Cookie")]
        [InlineData("password")]
        [InlineData("token")]
        [InlineData("secret")]
        [InlineData("apikey")]
        [InlineData("api_key")]
        public void Dado_DiversasChavesSensiveis_Quando_Finalizar_Entao_DeveRedigirTodas(string chave)
        {
            // Dado
            var processor = new PiiRedactionProcessor();
            using var activity = CriarActivity();
            activity.ShouldNotBeNull();
            activity.SetTag(chave, "valor-secreto");

            // Quando
            processor.OnEnd(activity);

            // Então
            activity.GetTagItem(chave).ShouldBe("[REDACTED]");
        }

        [Fact]
        public void Dado_TagComValorContendoPadraoSensivel_Quando_Finalizar_Entao_DeveRedigir()
        {
            // Dado
            var processor = new PiiRedactionProcessor();
            using var activity = CriarActivity();
            activity.ShouldNotBeNull();
            activity.SetTag("url.query", "user=admin&password=1234");

            // Quando
            processor.OnEnd(activity);

            // Então
            activity.GetTagItem("url.query").ShouldBe("[REDACTED]");
        }

        [Fact]
        public void Dado_TagNaoSensivel_Quando_Finalizar_Entao_DeveManterValor()
        {
            // Dado
            var processor = new PiiRedactionProcessor();
            using var activity = CriarActivity();
            activity.ShouldNotBeNull();
            activity.SetTag("http.method", "GET");

            // Quando
            processor.OnEnd(activity);

            // Então
            activity.GetTagItem("http.method").ShouldBe("GET");
        }
    }
}
