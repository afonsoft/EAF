using Eaf.Models.About;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.About
{
    public class AboutModelTests
    {
        [Fact]
        public void Dado_AboutModel_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var modules = new[] { "Module1", "Module2" };
            var environments = new Dictionary<string, string>
            {
                { "ASPNETCORE_ENVIRONMENT", "Development" },
                { "DB_HOST", "localhost" }
            };

            var model = new AboutModel
            {
                Version = "1.0.0",
                OSVersion = "Ubuntu 22.04",
                OS = "Linux",
                NumberOfProcessors = "8",
                MachineName = "server-01",
                Architecture = "x64",
                RuntimeIdentifier = "linux-x64",
                FrameworkDescription = ".NET 10.0",
                TotalAvailableMemory = "16 GB",
                CurrentCulture = "pt-BR",
                CurrentTimeZoneLocal = "America/Sao_Paulo",
                CurrentEnviromment = "Development",
                CurrentDirectory = "/app",
                ProcessName = "dotnet",
                PagedMemorySize = "100 MB",
                PrivateMemorySize = "200 MB",
                VirtualMemorySize = "1 GB",
                WorkingMemoryUsed = "150 MB",
                Modules = modules,
                Environments = environments
            };

            model.Version.ShouldBe("1.0.0");
            model.OSVersion.ShouldBe("Ubuntu 22.04");
            model.OS.ShouldBe("Linux");
            model.NumberOfProcessors.ShouldBe("8");
            model.MachineName.ShouldBe("server-01");
            model.Architecture.ShouldBe("x64");
            model.RuntimeIdentifier.ShouldBe("linux-x64");
            model.FrameworkDescription.ShouldBe(".NET 10.0");
            model.TotalAvailableMemory.ShouldBe("16 GB");
            model.CurrentCulture.ShouldBe("pt-BR");
            model.CurrentTimeZoneLocal.ShouldBe("America/Sao_Paulo");
            model.CurrentEnviromment.ShouldBe("Development");
            model.CurrentDirectory.ShouldBe("/app");
            model.ProcessName.ShouldBe("dotnet");
            model.PagedMemorySize.ShouldBe("100 MB");
            model.PrivateMemorySize.ShouldBe("200 MB");
            model.VirtualMemorySize.ShouldBe("1 GB");
            model.WorkingMemoryUsed.ShouldBe("150 MB");
            model.Modules.Length.ShouldBe(2);
            model.Environments.Count.ShouldBe(2);
        }

        [Fact]
        public void Dado_AboutModel_Quando_Instanciar_Entao_PropriedadesDevemSerNull()
        {
            var model = new AboutModel();
            model.Version.ShouldBeNull();
            model.OS.ShouldBeNull();
            model.Modules.ShouldBeNull();
            model.Environments.ShouldBeNull();
        }
    }
}
