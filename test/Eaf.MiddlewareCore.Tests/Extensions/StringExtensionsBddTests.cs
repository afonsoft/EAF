using Eaf.Middleware.StringExtensions;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Extensions
{
    /// <summary>
    /// Testes BDD para StringExtensions seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class StringExtensionsBddTests
    {
        #region FormatSize(long)

        [Fact]
        public void Dado_Zero_Quando_FormatSize_Entao_DeveRetornar0kB()
        {
            // Quando
            var result = 0L.FormatSize();

            // Então
            result.ShouldBe("0 kB");
        }

        [Fact]
        public void Dado_ValorNegativo_Quando_FormatSize_Entao_DeveRetornar0kB()
        {
            // Quando
            var result = (-100L).FormatSize();

            // Então
            result.ShouldBe("0 kB");
        }

        [Fact]
        public void Dado_MenosQue1024Bytes_Quando_FormatSize_Entao_DeveRetornar0kB()
        {
            // Quando
            var result = 500L.FormatSize();

            // Então
            result.ShouldBe("0 kB");
        }

        [Fact]
        public void Dado_1024Bytes_Quando_FormatSize_Entao_DeveRetornar1kB()
        {
            // Quando
            var result = 1024L.FormatSize();

            // Então
            result.ShouldBe("1 kB");
        }

        [Fact]
        public void Dado_1MB_Quando_FormatSize_Entao_DeveRetornar1MB()
        {
            // Dado
            long bytes = 1024L * 1024;

            // Quando
            var result = bytes.FormatSize();

            // Então
            result.ShouldBe("1 MB");
        }

        [Fact]
        public void Dado_1GB_Quando_FormatSize_Entao_DeveRetornar1GB()
        {
            // Dado
            long bytes = 1024L * 1024 * 1024;

            // Quando
            var result = bytes.FormatSize();

            // Então
            result.ShouldBe("1 GB");
        }

        [Fact]
        public void Dado_1TB_Quando_FormatSize_Entao_DeveRetornar1TB()
        {
            // Dado
            long bytes = 1024L * 1024 * 1024 * 1024;

            // Quando
            var result = bytes.FormatSize();

            // Então
            result.ShouldBe("1 TB");
        }

        [Fact]
        public void Dado_2500KB_Quando_FormatSize_Entao_DeveRetornar2MB()
        {
            // Dado
            long bytes = 2560L * 1024;

            // Quando
            var result = bytes.FormatSize();

            // Então
            result.ShouldBe("2 MB");
        }

        #endregion

        #region FormatSize(int)

        [Fact]
        public void Dado_IntZero_Quando_FormatSize_Entao_DeveRetornar0kB()
        {
            // Quando
            var result = 0.FormatSize();

            // Então
            result.ShouldBe("0 kB");
        }

        [Fact]
        public void Dado_Int1024_Quando_FormatSize_Entao_DeveRetornar1kB()
        {
            // Quando
            var result = 1024.FormatSize();

            // Então
            result.ShouldBe("1 kB");
        }

        #endregion

        #region IsContains

        [Fact]
        public void Dado_StringContemItem_Quando_VerificarIsContains_Entao_DeveRetornarTrue()
        {
            // Dado
            var text = "Hello World";

            // Quando
            var result = text.IsContains("Hello", "Bye");

            // Então
            result.ShouldBeTrue();
        }

        [Fact]
        public void Dado_StringNaoContemItem_Quando_VerificarIsContains_Entao_DeveRetornarFalse()
        {
            // Dado
            var text = "Hello World";

            // Quando
            var result = text.IsContains("Foo", "Bar");

            // Então
            result.ShouldBeFalse();
        }

        [Fact]
        public void Dado_StringComMultiplosMatches_Quando_VerificarIsContains_Entao_DeveRetornarTrue()
        {
            // Dado
            var text = "application/json";

            // Quando
            var result = text.IsContains("xml", "json", "csv");

            // Então
            result.ShouldBeTrue();
        }

        #endregion
    }
}
