using Eaf.Hangfire;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Hangfire
{
    /// <summary>
    /// Testes BDD para HangfireStorageType seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class HangfireStorageTypeBddTests
    {
        #region Valores do Enum

        [Fact]
        public void Dado_Enum_Quando_VerificarSqlServer_Entao_DeveExistir()
        {
            var value = HangfireStorageType.SqlServer;
            value.ShouldBe(HangfireStorageType.SqlServer);
        }

        [Fact]
        public void Dado_Enum_Quando_VerificarRedis_Entao_DeveExistir()
        {
            var value = HangfireStorageType.Redis;
            value.ShouldBe(HangfireStorageType.Redis);
        }

        [Fact]
        public void Dado_Enum_Quando_VerificarInMemory_Entao_DeveExistir()
        {
            var value = HangfireStorageType.InMemory;
            value.ShouldBe(HangfireStorageType.InMemory);
        }

        [Fact]
        public void Dado_Enum_Quando_ContarValores_Entao_DeveTerTresValores()
        {
            var values = Enum.GetValues(typeof(HangfireStorageType));
            values.Length.ShouldBe(3);
        }

        [Fact]
        public void Dado_SqlServer_Quando_ConverterParaInt_Entao_DeveSerZero()
        {
            ((int)HangfireStorageType.SqlServer).ShouldBe(0);
        }

        [Fact]
        public void Dado_Redis_Quando_ConverterParaInt_Entao_DeveSerUm()
        {
            ((int)HangfireStorageType.Redis).ShouldBe(1);
        }

        [Fact]
        public void Dado_InMemory_Quando_ConverterParaInt_Entao_DeveSerDois()
        {
            ((int)HangfireStorageType.InMemory).ShouldBe(2);
        }

        [Fact]
        public void Dado_StringSqlServer_Quando_Parse_Entao_DeveRetornarSqlServer()
        {
            Enum.Parse<HangfireStorageType>("SqlServer").ShouldBe(HangfireStorageType.SqlServer);
        }

        [Fact]
        public void Dado_StringRedis_Quando_Parse_Entao_DeveRetornarRedis()
        {
            Enum.Parse<HangfireStorageType>("Redis").ShouldBe(HangfireStorageType.Redis);
        }

        [Fact]
        public void Dado_StringInMemory_Quando_Parse_Entao_DeveRetornarInMemory()
        {
            Enum.Parse<HangfireStorageType>("InMemory").ShouldBe(HangfireStorageType.InMemory);
        }

        #endregion
    }
}
