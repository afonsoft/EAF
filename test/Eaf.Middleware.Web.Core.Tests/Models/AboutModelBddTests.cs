using Eaf.Models.About;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models
{
    /// <summary>
    /// Testes BDD para AboutModel seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class AboutModelBddTests
    {
        #region Propriedades

        [Fact]
        public void Dado_AboutModel_Quando_DefinirVersion_Entao_DeveArmazenarCorretamente()
        {
            var model = new AboutModel { Version = "10.0.0" };
            model.Version.ShouldBe("10.0.0");
        }

        [Fact]
        public void Dado_AboutModel_Quando_DefinirOSVersion_Entao_DeveArmazenarCorretamente()
        {
            var model = new AboutModel { OSVersion = "Ubuntu 22.04" };
            model.OSVersion.ShouldBe("Ubuntu 22.04");
        }

        [Fact]
        public void Dado_AboutModel_Quando_DefinirOS_Entao_DeveArmazenarCorretamente()
        {
            var model = new AboutModel { OS = "Linux" };
            model.OS.ShouldBe("Linux");
        }

        [Fact]
        public void Dado_AboutModel_Quando_DefinirNumberOfProcessors_Entao_DeveArmazenarCorretamente()
        {
            var model = new AboutModel { NumberOfProcessors = "8" };
            model.NumberOfProcessors.ShouldBe("8");
        }

        [Fact]
        public void Dado_AboutModel_Quando_DefinirMachineName_Entao_DeveArmazenarCorretamente()
        {
            var model = new AboutModel { MachineName = "server-01" };
            model.MachineName.ShouldBe("server-01");
        }

        [Fact]
        public void Dado_AboutModel_Quando_DefinirArchitecture_Entao_DeveArmazenarCorretamente()
        {
            var model = new AboutModel { Architecture = "x64" };
            model.Architecture.ShouldBe("x64");
        }

        [Fact]
        public void Dado_AboutModel_Quando_DefinirRuntimeIdentifier_Entao_DeveArmazenarCorretamente()
        {
            var model = new AboutModel { RuntimeIdentifier = "linux-x64" };
            model.RuntimeIdentifier.ShouldBe("linux-x64");
        }

        [Fact]
        public void Dado_AboutModel_Quando_DefinirFrameworkDescription_Entao_DeveArmazenarCorretamente()
        {
            var model = new AboutModel { FrameworkDescription = ".NET 10.0.0" };
            model.FrameworkDescription.ShouldBe(".NET 10.0.0");
        }

        [Fact]
        public void Dado_AboutModel_Quando_DefinirModules_Entao_DeveArmazenarCorretamente()
        {
            var modules = new[] { "Module1", "Module2" };
            var model = new AboutModel { Modules = modules };
            model.Modules.ShouldBe(modules);
        }

        [Fact]
        public void Dado_AboutModel_Quando_DefinirEnvironments_Entao_DeveArmazenarCorretamente()
        {
            var envs = new Dictionary<string, string> { { "KEY", "VALUE" } };
            var model = new AboutModel { Environments = envs };
            model.Environments.ShouldContainKeyAndValue("KEY", "VALUE");
        }

        [Fact]
        public void Dado_AboutModel_Quando_DefinirTotalAvailableMemory_Entao_DeveArmazenarCorretamente()
        {
            var model = new AboutModel { TotalAvailableMemory = "16 GB" };
            model.TotalAvailableMemory.ShouldBe("16 GB");
        }

        [Fact]
        public void Dado_AboutModel_Quando_DefinirCurrentCulture_Entao_DeveArmazenarCorretamente()
        {
            var model = new AboutModel { CurrentCulture = "pt-BR" };
            model.CurrentCulture.ShouldBe("pt-BR");
        }

        [Fact]
        public void Dado_AboutModel_Quando_DefinirProcessName_Entao_DeveArmazenarCorretamente()
        {
            var model = new AboutModel { ProcessName = "dotnet" };
            model.ProcessName.ShouldBe("dotnet");
        }

        [Fact]
        public void Dado_AboutModel_Quando_CriarInstancia_Entao_PropriedadesDevemSerNulas()
        {
            var model = new AboutModel();
            model.Version.ShouldBeNull();
            model.OS.ShouldBeNull();
            model.Modules.ShouldBeNull();
            model.Environments.ShouldBeNull();
        }

        #endregion
    }
}
