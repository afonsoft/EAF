using Eaf.Middleware.Worker.VirtualFileSystem;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.VirtualFileSystem
{
    public class PathUtilsTests
    {
        [Fact]
        public void Dado_CaminhoSemNavegacao_Quando_PathNavigatesAboveRoot_Entao_DeveRetornarFalse()
        {
            PathUtils.PathNavigatesAboveRoot("folder/subfolder/file.txt").ShouldBeFalse();
        }

        [Fact]
        public void Dado_CaminhoComPontosPontos_Quando_PathNavigatesAboveRoot_Entao_DeveRetornarTrue()
        {
            PathUtils.PathNavigatesAboveRoot("../etc/passwd").ShouldBeTrue();
        }

        [Fact]
        public void Dado_CaminhoComDoubleDotMasNaoAcimaRoot_Quando_PathNavigatesAboveRoot_Entao_DeveRetornarFalse()
        {
            PathUtils.PathNavigatesAboveRoot("folder/../file.txt").ShouldBeFalse();
        }

        [Fact]
        public void Dado_CaminhoComPontoSimples_Quando_PathNavigatesAboveRoot_Entao_DeveRetornarFalse()
        {
            PathUtils.PathNavigatesAboveRoot("./file.txt").ShouldBeFalse();
        }

        [Fact]
        public void Dado_CaminhoVazio_Quando_PathNavigatesAboveRoot_Entao_DeveRetornarFalse()
        {
            PathUtils.PathNavigatesAboveRoot("").ShouldBeFalse();
        }

        [Fact]
        public void Dado_CaminhoComMultiplosDoubleDots_Quando_PathNavigatesAboveRoot_Entao_DeveRetornarTrue()
        {
            PathUtils.PathNavigatesAboveRoot("a/b/../../..").ShouldBeTrue();
        }

        [Fact]
        public void Dado_CaminhoComMultiplosDoubleDotsDentroLimite_Quando_PathNavigatesAboveRoot_Entao_DeveRetornarFalse()
        {
            PathUtils.PathNavigatesAboveRoot("a/b/c/../../d").ShouldBeFalse();
        }

        [Fact]
        public void Dado_CaminhoProfundo_Quando_PathNavigatesAboveRoot_Entao_DeveRetornarFalse()
        {
            PathUtils.PathNavigatesAboveRoot("a/b/c/d/e/f/file.txt").ShouldBeFalse();
        }
    }
}
