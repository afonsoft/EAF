using Eaf.Runtime.Caching.Serialization;
using Shouldly;
using System;
using Xunit;

namespace Eaf.SqlServerCache.Tests.Serialization
{
    /// <summary>
    /// Testes de contrato para ICacheSerializer — garante que qualquer implementação respeita o contrato.
    /// </summary>
    public class ICacheSerializerContractTests
    {
        [Fact]
        public void Dado_Interface_Quando_VerificarMetodos_Entao_DeveConterSerializeEDeserialize()
        {
            // Dado
            var type = typeof(ICacheSerializer);

            // Quando
            var serializeMethod = type.GetMethod("Serialize");
            var deserializeMethod = type.GetMethod("Deserialize");

            // Então
            serializeMethod.ShouldNotBeNull();
            serializeMethod.ReturnType.ShouldBe(typeof(byte[]));
            serializeMethod.GetParameters().Length.ShouldBe(1);
            serializeMethod.GetParameters()[0].ParameterType.ShouldBe(typeof(object));

            deserializeMethod.ShouldNotBeNull();
            deserializeMethod.ReturnType.ShouldBe(typeof(object));
            deserializeMethod.GetParameters().Length.ShouldBe(1);
            deserializeMethod.GetParameters()[0].ParameterType.ShouldBe(typeof(byte[]));
        }

        [Fact]
        public void Dado_JsonCacheSerializer_Quando_VerificarImplementacao_Entao_DeveImplementarICacheSerializer()
        {
            // Dado & Quando
            var type = typeof(JsonCacheSerializer);

            // Então
            typeof(ICacheSerializer).IsAssignableFrom(type).ShouldBeTrue();
        }

        [Fact]
        public void Dado_JsonCacheSerializer_Quando_Instanciar_Entao_DeveFuncionar()
        {
            // Dado & Quando
            var serializer = new JsonCacheSerializer();

            // Então
            serializer.ShouldNotBeNull();
            serializer.ShouldBeAssignableTo<ICacheSerializer>();
        }
    }
}
