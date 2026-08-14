using System.IO;
using System.Threading.Tasks;
using Abp;
using Abp.BlobStoring;
using Shouldly;
using Xunit;

namespace Eaf.BlobStoring.Tests
{
    /// <summary>
    /// Testes integrados do contêiner de BLOBs com provedor FileSystem.
    /// </summary>
    public class FileSystemBlobContainerTests : BlobStoringTestBase
    {
        private readonly IBlobContainer _container;

        /// <summary>
        /// Inicializa o contêiner padrão.
        /// </summary>
        public FileSystemBlobContainerTests()
        {
            _container = Resolve<IBlobContainer>();
        }

        /// <summary>
        /// Dado um BLOB, quando salvar e recuperar os bytes, então o conteúdo deve ser o mesmo.
        /// </summary>
        [Fact]
        public async Task Dado_Blob_Quando_SalvarERecuperarBytes_Entao_Deve_RetornarMesmoConteudo()
        {
            var bytes = new byte[] { 1, 2, 3, 4, 5 };

            await _container.SaveAsync("meu-blob", bytes);
            var recuperado = await _container.GetAllBytesAsync("meu-blob");

            recuperado.ShouldBe(bytes);
        }

        /// <summary>
        /// Dado um BLOB existente, quando salvar sem sobrescrever, então deve lançar exceção de conflito.
        /// </summary>
        [Fact]
        public async Task Dado_BlobExistente_Quando_SalvarSemSobrescrever_Entao_Deve_LancarExcecaoDeConflito()
        {
            await _container.SaveAsync("blob", new byte[] { 1 });

            await Should.ThrowAsync<BlobAlreadyExistsException>(
                () => _container.SaveAsync("blob", new byte[] { 2 }));
        }

        /// <summary>
        /// Dado um BLOB existente, quando deletar, então ele não deve mais existir.
        /// </summary>
        [Fact]
        public async Task Dado_BlobExistente_Quando_Deletar_Entao_NaoDeveExistir()
        {
            await _container.SaveAsync("blob", new byte[] { 1 });

            await _container.DeleteAsync("blob");

            (await _container.ExistsAsync("blob")).ShouldBeFalse();
        }

        /// <summary>
        /// Dado um BLOB em subpasta, quando salvar, então deve criar hierarquia e recuperar corretamente.
        /// </summary>
        [Fact]
        public async Task Dado_BlobEmSubpasta_Quando_Salvar_Entao_Deve_CriarHierarquiaERetornarBytes()
        {
            var bytes = new byte[] { 9, 8, 7 };

            await _container.SaveAsync("pasta/arquivo.txt", bytes);
            var recuperado = await _container.GetAllBytesAsync("pasta/arquivo.txt");

            recuperado.ShouldBe(bytes);
        }

        /// <summary>
        /// Dado um BLOB inexistente, quando buscar, então deve lançar exceção.
        /// </summary>
        [Fact]
        public async Task Dado_BlobInexistente_Quando_Buscar_Entao_Deve_LancarExcecao()
        {
            await Should.ThrowAsync<AbpException>(async () => await _container.GetAsync("inexistente"));
        }

        /// <summary>
        /// Dado um nome com path traversal, quando salvar, então deve sanitizar e persistir.
        /// </summary>
        [Fact]
        public async Task Dado_NomeComPathTraversal_Quando_Salvar_Entao_Deve_SanitizarESalvarDentroDoDiretorio()
        {
            var bytes = new byte[] { 9 };

            await _container.SaveAsync("../../../etc/passwd", bytes);

            (await _container.ExistsAsync("../../../etc/passwd")).ShouldBeTrue();
        }
    }
}
