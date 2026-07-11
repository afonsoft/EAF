using Eaf.Middleware.Web;
using Shouldly;
using System;
using System.IO;
using Xunit;

namespace Eaf.Middleware.Tests.Net.Web
{
    public class WebContentDirectoryFinderBddTests
    {
        [Fact]
        public void Dado_AssemblyCore_Quando_CalculateContentRootFolder_Entao_DeveLancarExcecaoSeWebHostNaoExistir()
        {
            var exception = Assert.Throws<Exception>(() => WebContentDirectoryFinder.CalculateContentRootFolder());

            exception.Message.ShouldContain("Could not find root folder of the web project");
        }

        [Fact]
        public void Dado_AssemblyCore_Quando_CalculateContentRootFolder_Entao_DeveEncontrarEafSln()
        {
            var exception = Assert.Throws<Exception>(() => WebContentDirectoryFinder.CalculateContentRootFolder());

            exception.Message.ShouldContain("Could not find root folder of the web project");
        }
    }
}
