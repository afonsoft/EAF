using Eaf.BlobStoring.Naming;
using Shouldly;
using Xunit;

namespace Eaf.BlobStoring.Tests
{
    /// <summary>
    /// Testes do normalizador padrão de nomes de BLOBs.
    /// </summary>
    public class EafDefaultBlobNamingNormalizerTests
    {
        private readonly EafDefaultBlobNamingNormalizer _normalizer;

        /// <summary>
        /// Inicializa o normalizador.
        /// </summary>
        public EafDefaultBlobNamingNormalizerTests()
        {
            _normalizer = new EafDefaultBlobNamingNormalizer();
        }

        /// <summary>
        /// Dado um nome de contêiner com maiúsculas, quando normalizar, então deve retornar minúsculas.
        /// </summary>
        [Fact]
        public void Dado_NomeContainerComMaiusculas_Quando_Normalizar_Entao_Deve_RetornarMinusculas()
        {
            _normalizer.NormalizeContainerName("MeuContainer").ShouldBe("meucontainer");
        }

        /// <summary>
        /// Dado um nome de contêiner com caracteres inválidos, quando normalizar, então deve remover ou substituir.
        /// </summary>
        [Fact]
        public void Dado_NomeContainerComCaracteresInvalidos_Quando_Normalizar_Entao_Deve_AjustarNome()
        {
            _normalizer.NormalizeContainerName("meu@container#1").ShouldBe("meu-container-1");
        }

        /// <summary>
        /// Dado um nome de contêiner muito curto, quando normalizar, então deve completar até 3 caracteres.
        /// </summary>
        [Fact]
        public void Dado_NomeContainerMuitoCurto_Quando_Normalizar_Entao_Deve_CompletarAte3Caracteres()
        {
            _normalizer.NormalizeContainerName("a").ShouldBe("a00");
        }

        /// <summary>
        /// Dado um nome de BLOB com segmentos inválidos, quando normalizar, então deve remover os segmentos.
        /// </summary>
        [Fact]
        public void Dado_NomeBlobComPontos_Quando_Normalizar_Entao_Deve_RemoverSegmentosInvalidos()
        {
            _normalizer.NormalizeBlobName("pasta/../arquivo.txt").ShouldBe("pasta/arquivo.txt");
        }

        /// <summary>
        /// Dado um nome de BLOB com backslash, quando normalizar, então deve converter para barra normal.
        /// </summary>
        [Fact]
        public void Dado_NomeBlobComBackslash_Quando_Normalizar_Entao_Deve_ConverterParaBarraNormal()
        {
            _normalizer.NormalizeBlobName("pasta\\arquivo.txt").ShouldBe("pasta/arquivo.txt");
        }
    }
}
