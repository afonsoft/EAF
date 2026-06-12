using Eaf.Middleware.Storage;
using Shouldly;
using System;
using System.Text;
using Xunit;

namespace Eaf.Middleware.Tests.Storage
{
    /// <summary>
    /// Testes BDD para BinaryObject seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class BinaryObjectBddTests
    {
        [Fact]
        public void Dado_ConstrutorPadrao_Quando_Criar_Entao_DeveGerarId()
        {
            var obj = new BinaryObject();
            obj.Id.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public void Dado_ConstrutorComParametros_Quando_Criar_Entao_DeveDefinirPropriedades()
        {
            var bytes = Encoding.UTF8.GetBytes("conteúdo de teste");
            var obj = new BinaryObject(1, bytes, "text/plain", "arquivo.txt");

            obj.TenantId.ShouldBe(1);
            obj.Bytes.ShouldBe(bytes);
            obj.FileType.ShouldBe("text/plain");
            obj.FileName.ShouldContain("arquivo.txt");
            obj.Id.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public void Dado_ConstrutorComTenantNull_Quando_Criar_Entao_TenantDeveSerNull()
        {
            var bytes = new byte[] { 1, 2, 3 };
            var obj = new BinaryObject(null, bytes, "application/octet-stream", "data.bin");

            obj.TenantId.ShouldBeNull();
            obj.FileType.ShouldBe("application/octet-stream");
        }

        [Fact]
        public void Dado_BinaryObject_Quando_CriarDoisObjetos_Entao_IdsDevemSerDiferentes()
        {
            var obj1 = new BinaryObject();
            var obj2 = new BinaryObject();

            obj1.Id.ShouldNotBe(obj2.Id);
        }

        [Fact]
        public void Dado_BinaryObject_Quando_CriarComFileName_Entao_FileNameDeveConterIdEOriginal()
        {
            var obj = new BinaryObject(1, new byte[] { 0 }, "img/png", "foto.png");
            obj.FileName.ShouldContain("foto.png");
            obj.FileName.ShouldContain("_");
        }
    }
}
