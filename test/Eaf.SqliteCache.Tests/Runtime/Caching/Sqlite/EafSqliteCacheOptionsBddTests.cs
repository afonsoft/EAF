using Abp.Runtime.Caching.Sqlite;
using Shouldly;
using System;
using Xunit;

namespace Eaf.SqliteCache.Tests.Runtime.Caching.Sqlite
{
    public class EafSqliteCacheOptionsBddTests
    {
        [Fact]
        public void Dado_CachePathNulo_Quando_Definir_Entao_DeveSerVazio()
        {
            var options = new EafSqliteCacheOptions();
            options.CachePath = null!;
            options.CachePath.ShouldBeEmpty();
        }

        [Fact]
        public void Dado_CachePathDataSource_Quando_Definir_Entao_DeveExtrairCaminho()
        {
            var options = new EafSqliteCacheOptions();
            options.CachePath = "Data Source=foo.db";
            options.CachePath.ShouldBe("foo.db");
        }

        [Fact]
        public void Dado_CachePathComChaveValor_Quando_Definir_Entao_DeveLancarArgumentException()
        {
            var options = new EafSqliteCacheOptions();
            Should.Throw<ArgumentException>(() => options.CachePath = "key=value");
        }

        [Fact]
        public void Dado_CachePathDataSourceComChaveValor_Quando_Definir_Entao_DeveLancarArgumentException()
        {
            var options = new EafSqliteCacheOptions();
            Should.Throw<ArgumentException>(() => options.CachePath = "Data Source=key=value");
        }

        [Fact]
        public void Dado_CachePathRelativoComIgual_Quando_Definir_Entao_DeveManterCaminho()
        {
            var options = new EafSqliteCacheOptions();
            options.CachePath = "./path=value";
            options.CachePath.ShouldBe("./path=value");
        }

        [Fact]
        public void Dado_CachePathAbsolutoComIgual_Quando_Definir_Entao_DeveManterCaminho()
        {
            var options = new EafSqliteCacheOptions();
            options.CachePath = "/path=value";
            options.CachePath.ShouldBe("/path=value");
        }

        [Fact]
        public void Dado_CachePathComDrive_Quando_Definir_Entao_DeveManterCaminho()
        {
            var options = new EafSqliteCacheOptions();
            options.CachePath = "C:\\path=value";
            options.CachePath.ShouldBe("C:\\path=value");
        }

        [Fact]
        public void Dado_CachePathComExtensaoDb_Quando_Definir_Entao_DeveManterCaminho()
        {
            var options = new EafSqliteCacheOptions();
            options.CachePath = "path=value.db";
            options.CachePath.ShouldBe("path=value.db");
        }

        [Fact]
        public void Dado_MemoryOnlyVerdadeiro_Quando_ConnectionString_Entao_DeveSerMemory()
        {
            var options = new EafSqliteCacheOptions
            {
                CachePath = "foo.db",
                MemoryOnly = true
            };

            options.ConnectionString.ShouldContain(":memory:");
            options.ConnectionString.ShouldContain("Mode=Memory");
        }

        [Fact]
        public void Dado_MemoryOnlyFalso_Quando_ConnectionString_Entao_DeveConterCaminho()
        {
            var options = new EafSqliteCacheOptions
            {
                CachePath = "foo.db",
                MemoryOnly = false
            };

            options.ConnectionString.ShouldContain("Data Source=foo.db");
            options.ConnectionString.ShouldContain("Mode=ReadWriteCreate");
        }
    }
}
