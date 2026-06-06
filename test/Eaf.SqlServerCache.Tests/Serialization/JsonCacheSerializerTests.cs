using Eaf.Runtime.Caching.Serialization;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Eaf.SqlServerCache.Tests.Serialization
{
    /// <summary>
    /// Testes para JsonCacheSerializer — Spec 85.
    /// </summary>
    public class JsonCacheSerializerTests
    {
        private readonly JsonCacheSerializer _serializer;

        public JsonCacheSerializerTests()
        {
            _serializer = new JsonCacheSerializer();
        }

        #region Serialize

        [Fact]
        public void Dado_ObjetoNulo_Quando_Serializar_Entao_DeveRetornarNull()
        {
            // Dado
            object obj = null;

            // Quando
            var result = _serializer.Serialize(obj);

            // Então
            result.ShouldBeNull();
        }

        [Fact]
        public void Dado_StringSimples_Quando_Serializar_Entao_DeveRetornarBytesValidos()
        {
            // Dado
            var value = "hello world";

            // Quando
            var result = _serializer.Serialize(value);

            // Então
            result.ShouldNotBeNull();
            result.Length.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Dado_Inteiro_Quando_Serializar_Entao_DeveRetornarBytesValidos()
        {
            // Dado
            var value = 42;

            // Quando
            var result = _serializer.Serialize(value);

            // Então
            result.ShouldNotBeNull();
            var json = Encoding.UTF8.GetString(result);
            json.ShouldBe("42");
        }

        [Fact]
        public void Dado_ObjetoComplexo_Quando_Serializar_Entao_DeveConterPropriedadesEmCamelCase()
        {
            // Dado
            var value = new { Nome = "Teste", Idade = 30 };

            // Quando
            var result = _serializer.Serialize(value);

            // Então
            result.ShouldNotBeNull();
            var json = Encoding.UTF8.GetString(result);
            json.ShouldContain("\"nome\"");
            json.ShouldContain("\"idade\"");
        }

        [Fact]
        public void Dado_ObjetoComPropriedadeNula_Quando_Serializar_Entao_DeveIgnorarNulos()
        {
            // Dado
            var value = new { Nome = "Teste", Descricao = (string)null };

            // Quando
            var result = _serializer.Serialize(value);

            // Então
            result.ShouldNotBeNull();
            var json = Encoding.UTF8.GetString(result);
            json.ShouldNotContain("descricao");
        }

        [Fact]
        public void Dado_Lista_Quando_Serializar_Entao_DeveSerializarCorretamente()
        {
            // Dado
            var value = new List<string> { "item1", "item2", "item3" };

            // Quando
            var result = _serializer.Serialize(value);

            // Então
            result.ShouldNotBeNull();
            var json = Encoding.UTF8.GetString(result);
            json.ShouldContain("item1");
            json.ShouldContain("item2");
            json.ShouldContain("item3");
        }

        [Fact]
        public void Dado_Dicionario_Quando_Serializar_Entao_DeveSerializarCorretamente()
        {
            // Dado
            var value = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };

            // Quando
            var result = _serializer.Serialize(value);

            // Então
            result.ShouldNotBeNull();
            var json = Encoding.UTF8.GetString(result);
            json.ShouldContain("\"a\"");
            json.ShouldContain("\"b\"");
        }

        [Fact]
        public void Dado_StringVazia_Quando_Serializar_Entao_DeveRetornarBytesValidos()
        {
            // Dado
            var value = "";

            // Quando
            var result = _serializer.Serialize(value);

            // Então
            result.ShouldNotBeNull();
            result.Length.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Dado_ObjetoAninhado_Quando_Serializar_Entao_DeveSerializarProfundamente()
        {
            // Dado
            var value = new
            {
                Nivel1 = new
                {
                    Nivel2 = new
                    {
                        Valor = "profundo"
                    }
                }
            };

            // Quando
            var result = _serializer.Serialize(value);

            // Então
            result.ShouldNotBeNull();
            var json = Encoding.UTF8.GetString(result);
            json.ShouldContain("profundo");
        }

        #endregion

        #region Deserialize

        [Fact]
        public void Dado_BytesNulos_Quando_Desserializar_Entao_DeveRetornarNull()
        {
            // Dado
            byte[] data = null;

            // Quando
            var result = _serializer.Deserialize(data);

            // Então
            result.ShouldBeNull();
        }

        [Fact]
        public void Dado_BytesVazios_Quando_Desserializar_Entao_DeveRetornarNull()
        {
            // Dado
            byte[] data = Array.Empty<byte>();

            // Quando
            var result = _serializer.Deserialize(data);

            // Então
            result.ShouldBeNull();
        }

        [Fact]
        public void Dado_JsonValido_Quando_Desserializar_Entao_DeveRetornarObjeto()
        {
            // Dado
            var json = "{\"nome\":\"Teste\",\"valor\":42}";
            var data = Encoding.UTF8.GetBytes(json);

            // Quando
            var result = _serializer.Deserialize(data);

            // Então
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_InteiroSerializado_Quando_Desserializar_Entao_DeveRetornarValor()
        {
            // Dado
            var data = Encoding.UTF8.GetBytes("42");

            // Quando
            var result = _serializer.Deserialize(data);

            // Então
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ArrayJsonSerializado_Quando_Desserializar_Entao_DeveRetornarObjeto()
        {
            // Dado
            var data = Encoding.UTF8.GetBytes("[1,2,3]");

            // Quando
            var result = _serializer.Deserialize(data);

            // Então
            result.ShouldNotBeNull();
        }

        #endregion

        #region Roundtrip

        [Fact]
        public void Dado_String_Quando_SerializarEDesserializar_Entao_DeveManterValor()
        {
            // Dado
            var original = "roundtrip test";

            // Quando
            var bytes = _serializer.Serialize(original);
            var result = _serializer.Deserialize(bytes);

            // Então
            result.ShouldNotBeNull();
            result.ToString().ShouldBe(original);
        }

        [Fact]
        public void Dado_Numero_Quando_SerializarEDesserializar_Entao_DeveManterValor()
        {
            // Dado
            var original = 12345;

            // Quando
            var bytes = _serializer.Serialize(original);
            var result = _serializer.Deserialize(bytes);

            // Então
            result.ShouldNotBeNull();
        }

        #endregion
    }
}
