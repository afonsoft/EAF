using Eaf.Middleware.Storage;
using Shouldly;
using System;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Storage
{
    public class BinaryObjectTests
    {
        [Fact]
        public void Dado_ConstrutorPadrao_Quando_CriarBinaryObject_Entao_DeveGerarIdSequencial()
        {
            var obj = new BinaryObject();
            obj.Id.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public void Dado_ParametrosValidos_Quando_CriarBinaryObject_Entao_DeveDefinirPropriedades()
        {
            var bytes = new byte[] { 0x01, 0x02, 0x03 };
            var obj = new BinaryObject(1, bytes, "application/pdf", "document.pdf");

            obj.TenantId.ShouldBe(1);
            obj.Bytes.ShouldBe(bytes);
            obj.FileType.ShouldBe("application/pdf");
            obj.FileName.ShouldContain("document.pdf");
            obj.Id.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public void Dado_TenantIdNull_Quando_CriarBinaryObject_Entao_DevePermitir()
        {
            var bytes = new byte[] { 0x01 };
            var obj = new BinaryObject(null, bytes, "text/plain", "test.txt");

            obj.TenantId.ShouldBeNull();
            obj.FileType.ShouldBe("text/plain");
        }

        [Fact]
        public void Dado_BinaryObject_Quando_VerificarFileName_Entao_DeveConterIdSemHifens()
        {
            var bytes = new byte[] { 0x01 };
            var obj = new BinaryObject(1, bytes, "text/plain", "file.txt");

            obj.FileName.ShouldNotContain("-_");
            obj.FileName.ShouldEndWith("file.txt");
        }

        [Fact]
        public void Dado_DoisBinaryObjects_Quando_CriarSeparadamente_Entao_DevemTerIdsDistintos()
        {
            var obj1 = new BinaryObject();
            var obj2 = new BinaryObject();

            obj1.Id.ShouldNotBe(obj2.Id);
        }
    }
}
