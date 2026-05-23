using Eaf.Hangfire;
using Shouldly;
using System;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Hangfire
{
    public class HangfireStorageTypeTests
    {
        [Fact]
        public void Dado_Enum_Quando_VerificarValores_Entao_DeveConterTresTipos()
        {
            var values = Enum.GetValues(typeof(HangfireStorageType));
            values.Length.ShouldBe(3);
        }

        [Fact]
        public void Dado_SqlServer_Quando_Converter_Entao_DeveSerZero()
        {
            ((int)HangfireStorageType.SqlServer).ShouldBe(0);
        }

        [Fact]
        public void Dado_Redis_Quando_Converter_Entao_DeveSerUm()
        {
            ((int)HangfireStorageType.Redis).ShouldBe(1);
        }

        [Fact]
        public void Dado_InMemory_Quando_Converter_Entao_DeveSerDois()
        {
            ((int)HangfireStorageType.InMemory).ShouldBe(2);
        }

        [Fact]
        public void Dado_Enum_Quando_VerificarNomes_Entao_DeveConterTodosOsNomes()
        {
            var names = Enum.GetNames(typeof(HangfireStorageType));
            names.ShouldContain("SqlServer");
            names.ShouldContain("Redis");
            names.ShouldContain("InMemory");
        }
    }
}
