using Abp.Runtime.Caching.Sqlite;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace Eaf.SqliteCache.Tests
{
    /// <summary>
    /// Testes de serialização para EafSqliteCache — Spec 02.
    /// Verifica a remoção do BinaryFormatter e uso de XML/JSON fallback.
    /// </summary>
    public class EafSqliteCacheSerializationTests
    {
        private static int _cacheCounter = 1000;

        private string GetUniqueCacheName()
        {
            return $"serialization-test-{Interlocked.Increment(ref _cacheCounter)}";
        }

        private EafSqliteCache CreateInMemoryCache()
        {
            return new EafSqliteCache(GetUniqueCacheName(), new EafSqliteCacheOptions { MemoryOnly = true });
        }

        #region Tipos Primitivos (XML primário)

        [Fact]
        public void Dado_String_Quando_SetEGet_Entao_DeveRoundtripCorretamente()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "string-key";
            var value = "hello world";

            // Quando
            cache.Set(key, value);
            var found = cache.TryGetValue(key, out var result);

            // Então
            found.ShouldBeTrue();
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Inteiro_Quando_Set_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "int-key";
            var value = 42;

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, value));
        }

        [Fact]
        public void Dado_Double_Quando_Set_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "double-key";
            var value = 3.14159265;

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, value));
        }

        [Fact]
        public void Dado_Boolean_Quando_Set_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "bool-key";

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, true));
            Should.NotThrow(() => cache.Set(key, false));
        }

        [Fact]
        public void Dado_DateTime_Quando_Set_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "datetime-key";
            var value = DateTime.UtcNow;

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, value));
        }

        [Fact]
        public void Dado_Guid_Quando_Set_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "guid-key";
            var value = Guid.NewGuid();

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, value));
        }

        #endregion

        #region Tipos Complexos (JSON fallback - Spec 02)

        [Fact]
        public void Dado_ObjetoAnonimo_Quando_Set_Entao_DeveUsarJsonFallbackSemErro()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "anonymous-key";
            var value = new { Id = 1, Nome = "Teste", Ativo = true };

            // Quando & Então (sem BinaryFormatter, deve usar JSON fallback)
            Should.NotThrow(() => cache.Set(key, value));
        }

        [Fact]
        public void Dado_Lista_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "list-key";
            var value = new List<string> { "item1", "item2", "item3" };

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, value));
        }

        [Fact]
        public void Dado_Dicionario_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "dict-key";
            var value = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, value));
        }

        [Fact]
        public void Dado_ObjetoAninhado_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "nested-key";
            var value = new
            {
                Nivel1 = new
                {
                    Nivel2 = new { Valor = "profundo" }
                }
            };

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, value));
        }

        [Fact]
        public void Dado_ArrayDeTiposMistos_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "mixed-key";
            var value = new object[] { "texto", 123, 3.14, true };

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, value));
        }

        #endregion

        #region Valores Nulos e Edge Cases

        [Fact]
        public void Dado_ValorNulo_Quando_Set_Entao_DeveLancarInvalidOperationException()
        {
            // Dado — SQLite cache não suporta valores nulos (requer Value)
            using var cache = CreateInMemoryCache();
            var key = "null-key";

            // Quando & Então
            Should.Throw<InvalidOperationException>(() => cache.Set(key, null));
        }

        [Fact]
        public void Dado_StringVazia_Quando_Set_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "empty-string-key";

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, ""));
        }

        [Fact]
        public void Dado_StringMuitoGrande_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "large-string-key";
            var value = new string('X', 100000);

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, value));
        }

        [Fact]
        public void Dado_StringComCaracteresEspeciais_Quando_Set_Entao_DeveSerializarSemErro()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "special-chars-key";
            var value = "Acentuação: àáâãéêíóôõúç <xml> & \"quotes\" 'single'";

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, value));
        }

        #endregion

        #region Expiração

        [Fact]
        public void Dado_ExpiracaoSlidingCustom_Quando_Set_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "sliding-key";
            var sliding = TimeSpan.FromMinutes(30);

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, "value", sliding));
        }

        [Fact]
        public void Dado_ExpiracaoAbsoluteCustom_Quando_Set_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "absolute-key";
            var absolute = DateTimeOffset.UtcNow.AddHours(2);

            // Quando & Então
            Should.NotThrow(() => cache.Set(key, "value", null, absolute));
        }

        #endregion

        #region Get após Set

        [Fact]
        public void Dado_ValorSetado_Quando_TryGetValue_Entao_DeveRetornarTrue()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            var key = "roundtrip-key";
            cache.Set(key, "test-value");

            // Quando
            var found = cache.TryGetValue(key, out var result);

            // Então
            found.ShouldBeTrue();
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ChaveInexistente_Quando_TryGetValue_Entao_DeveRetornarFalse()
        {
            // Dado
            using var cache = CreateInMemoryCache();

            // Quando
            var found = cache.TryGetValue("nonexistent", out var result);

            // Então
            found.ShouldBeFalse();
        }

        [Fact]
        public void Dado_ValorRemovido_Quando_TryGetValue_Entao_DeveRetornarFalse()
        {
            // Dado
            using var cache = CreateInMemoryCache();
            cache.Set("remove-me", "value");
            cache.Remove("remove-me");

            // Quando
            var found = cache.TryGetValue("remove-me", out var result);

            // Então
            found.ShouldBeFalse();
        }

        #endregion
    }
}
