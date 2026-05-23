using Eaf.Middleware.StringExtensions;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void Dado_ValorZero_Quando_FormatSize_Entao_DeveRetornar0KB()
        {
            0L.FormatSize().ShouldBe("0 kB");
        }

        [Fact]
        public void Dado_ValorNegativo_Quando_FormatSize_Entao_DeveRetornar0KB()
        {
            (-100L).FormatSize().ShouldBe("0 kB");
        }

        [Fact]
        public void Dado_ValorMenorQue1024_Quando_FormatSize_Entao_DeveRetornar0KB()
        {
            500L.FormatSize().ShouldBe("0 kB");
        }

        [Fact]
        public void Dado_1024Bytes_Quando_FormatSize_Entao_DeveRetornar1KB()
        {
            1024L.FormatSize().ShouldBe("1 kB");
        }

        [Fact]
        public void Dado_1MBEmBytes_Quando_FormatSize_Entao_DeveRetornar1MB()
        {
            (1024L * 1024).FormatSize().ShouldBe("1 MB");
        }

        [Fact]
        public void Dado_1GBEmBytes_Quando_FormatSize_Entao_DeveRetornar1GB()
        {
            (1024L * 1024 * 1024).FormatSize().ShouldBe("1 GB");
        }

        [Fact]
        public void Dado_IntZero_Quando_FormatSize_Entao_DeveRetornar0KB()
        {
            0.FormatSize().ShouldBe("0 kB");
        }

        [Fact]
        public void Dado_IntPositivo_Quando_FormatSize_Entao_DeveRetornarFormatado()
        {
            (1024 * 1024).FormatSize().ShouldBe("1 MB");
        }

        [Fact]
        public void Dado_StringContendoItem_Quando_IsContains_Entao_DeveRetornarTrue()
        {
            "hello world".IsContains("hello", "test").ShouldBeTrue();
        }

        [Fact]
        public void Dado_StringNaoContendoItem_Quando_IsContains_Entao_DeveRetornarFalse()
        {
            "hello world".IsContains("foo", "bar").ShouldBeFalse();
        }

        [Fact]
        public void Dado_StringContendoUmDosItens_Quando_IsContains_Entao_DeveRetornarTrue()
        {
            "test string".IsContains("abc", "string", "xyz").ShouldBeTrue();
        }
    }
}
