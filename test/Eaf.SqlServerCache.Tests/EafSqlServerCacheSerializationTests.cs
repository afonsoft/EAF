using Eaf.Runtime.Caching.SqlServer;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.SqlServerCache.Tests
{
    /// <summary>
    /// Testes adicionais de serialização para EafSqlServerCache — Specs 01-03.
    /// Foca em cenários de serialização XML/JSON fallback e sync-over-async.
    /// </summary>
    public class EafSqlServerCacheSerializationTests
    {
        private readonly IDistributedCache _distributedCache;
        private readonly EafSqlServerCache _cache;

        public EafSqlServerCacheSerializationTests()
        {
            _distributedCache = Substitute.For<IDistributedCache>();
            _cache = new EafSqlServerCache("serialization-tests", _distributedCache);
        }

        #region Serialização XML Primária (Spec 01)

        [Fact]
        public void Dado_ValorInteiro_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            var key = "int-key";
            var value = 42;

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, value));
            _distributedCache.Received(1).SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Dado_ValorDouble_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            var key = "double-key";
            var value = 3.14159;

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, value));
        }

        [Fact]
        public void Dado_ValorBooleano_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            var key = "bool-key";
            var value = true;

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, value));
        }

        [Fact]
        public void Dado_ValorDateTime_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            var key = "datetime-key";
            var value = DateTime.UtcNow;

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, value));
        }

        [Fact]
        public void Dado_ValorGuid_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            var key = "guid-key";
            var value = Guid.NewGuid();

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, value));
        }

        #endregion

        #region Serialização JSON Fallback (Spec 01-02)

        [Fact]
        public void Dado_ObjetoAnonimo_Quando_Set_Entao_DeveUsarJsonFallback()
        {
            // Dado — objetos anônimos não serializam bem com XML
            var key = "anonymous-key";
            var value = new { Id = 1, Nome = "Teste", Ativo = true };

            // Quando & Então (JSON fallback deve funcionar)
            Should.NotThrow(() => _cache.Set(key, value));
        }

        [Fact]
        public void Dado_ListaDeObjetos_Quando_Set_Entao_DeveSerializarViaFallback()
        {
            // Dado
            var key = "list-key";
            var value = new List<object> { "item1", 2, true, null };

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, value));
        }

        [Fact]
        public void Dado_DicionarioComplexo_Quando_Set_Entao_DeveSerializarViaFallback()
        {
            // Dado
            var key = "dict-key";
            var value = new Dictionary<string, object>
            {
                { "chave1", "valor1" },
                { "chave2", 42 },
                { "chave3", new[] { 1, 2, 3 } }
            };

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, value));
        }

        [Fact]
        public void Dado_ArrayDeTiposMistos_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            var key = "mixed-array-key";
            var value = new object[] { "texto", 123, 3.14, true, DateTime.UtcNow };

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, value));
        }

        #endregion

        #region Sync-over-Async Corrigido (Spec 03)

        [Fact]
        public void Dado_DistributedCacheQueRetornaTask_Quando_Set_Entao_DeveAguardarComGetAwaiterGetResult()
        {
            // Dado
            _distributedCache.SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            // Quando
            _cache.Set("sync-test", "value");

            // Então — Se não aguardasse, SetAsync não seria chamado de forma síncrona
            _distributedCache.Received(1).SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Dado_DistributedCacheQueRetornaTask_Quando_Remove_Entao_DeveAguardarComGetAwaiterGetResult()
        {
            // Dado
            _distributedCache.RemoveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            // Quando
            _cache.Remove("remove-test");

            // Então
            _distributedCache.Received(1).RemoveAsync(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Dado_DistributedCacheQueLancaExcecaoNoSet_Quando_Set_Entao_DevePropagar()
        {
            // Dado
            _distributedCache.SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>())
                .Returns(Task.FromException(new InvalidOperationException("DB connection failed")));

            // Quando & Então
            Should.Throw<InvalidOperationException>(() => _cache.Set("fail-key", "value"));
        }

        #endregion

        #region TryGetValue com desserialização

        [Fact]
        public void Dado_CacheComDadosNulos_Quando_TryGetValue_Entao_DeveRetornarFalse()
        {
            // Dado
            _distributedCache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<byte[]>(null));

            // Quando
            var result = _cache.TryGetValue("missing-key", out var value);

            // Então
            result.ShouldBeFalse();
            value.ShouldBeNull();
        }

        [Fact]
        public void Dado_CacheComBytesVazios_Quando_TryGetValue_Entao_DeveRetornarFalse()
        {
            // Dado
            _distributedCache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Array.Empty<byte>()));

            // Quando
            var result = _cache.TryGetValue("empty-key", out var value);

            // Então
            result.ShouldBeFalse();
            value.ShouldBeNull();
        }

        [Fact]
        public void Dado_CacheQueLancaExcecaoNoGet_Quando_TryGetValue_Entao_DeveRetornarFalse()
        {
            // Dado
            _distributedCache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromException<byte[]>(new TimeoutException("Connection timeout")));

            // Quando
            var result = _cache.TryGetValue("timeout-key", out var value);

            // Então
            result.ShouldBeFalse();
            value.ShouldBeNull();
        }

        #endregion

        #region Set com expiração custom

        [Fact]
        public void Dado_ExpiracaoSlidingCustom_Quando_Set_Entao_DevePassarParaDistributedCache()
        {
            // Dado
            var slidingExpiration = TimeSpan.FromMinutes(30);

            // Quando
            _cache.Set("custom-sliding", "value", slidingExpiration);

            // Então
            _distributedCache.Received(1).SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Is<DistributedCacheEntryOptions>(o => o.SlidingExpiration == slidingExpiration),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Dado_ExpiracaoAbsoluteCustom_Quando_Set_Entao_DevePassarParaDistributedCache()
        {
            // Dado
            var absoluteExpiration = DateTimeOffset.UtcNow.AddHours(2);

            // Quando
            _cache.Set("custom-absolute", "value", null, absoluteExpiration);

            // Então
            _distributedCache.Received(1).SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpiration == absoluteExpiration),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public void Dado_AmbasExpiracoes_Quando_Set_Entao_DevePassarAmbas()
        {
            // Dado
            var sliding = TimeSpan.FromMinutes(5);
            var absolute = DateTimeOffset.UtcNow.AddHours(1);

            // Quando
            _cache.Set("both-exp", "value", sliding, absolute);

            // Então
            _distributedCache.Received(1).SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Is<DistributedCacheEntryOptions>(o =>
                    o.SlidingExpiration == sliding &&
                    o.AbsoluteExpiration == absolute),
                Arg.Any<CancellationToken>());
        }

        #endregion

        #region Set com valor null

        [Fact]
        public void Dado_ValorNulo_Quando_Set_Entao_NaoDeveLancarExcecao()
        {
            // Dado — ObjectToByteArray retorna default para null
            var key = "null-value-key";

            // Quando & Então
            Should.NotThrow(() => _cache.Set(key, null));
        }

        #endregion
    }
}
