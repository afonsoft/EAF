using Eaf.Models.About;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Models.About
{
    /// <summary>
    /// Testes BDD para AboutModel seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AboutModelBddTests
    {
        [Fact]
        public void Dado_AboutModel_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var model = new AboutModel
            {
                Version = "10.0.0",
                OSVersion = "Ubuntu 22.04",
                OS = "Linux",
                NumberOfProcessors = "8",
                MachineName = "prod-server-01",
                Architecture = "X64",
                RuntimeIdentifier = "linux-x64",
                FrameworkDescription = ".NET 10.0.0",
                TotalAvailableMemory = "16 GB",
                CurrentCulture = "pt-BR",
                CurrentTimeZoneLocal = "America/Sao_Paulo",
                CurrentEnviromment = "Production",
                CurrentDirectory = "/app",
                ProcessName = "dotnet",
                PagedMemorySize = "100 MB",
                PrivateMemorySize = "200 MB",
                VirtualMemorySize = "500 MB",
                WorkingMemoryUsed = "150 MB",
                Modules = new[] { "Eaf.Core", "Eaf.Web" },
                Environments = new Dictionary<string, string>
                {
                    { "ASPNETCORE_ENVIRONMENT", "Production" },
                    { "DOTNET_RUNNING_IN_CONTAINER", "true" }
                }
            };

            model.Version.ShouldBe("10.0.0");
            model.OS.ShouldBe("Linux");
            model.Architecture.ShouldBe("X64");
            model.RuntimeIdentifier.ShouldBe("linux-x64");
            model.FrameworkDescription.ShouldBe(".NET 10.0.0");
            model.CurrentCulture.ShouldBe("pt-BR");
            model.ProcessName.ShouldBe("dotnet");
            model.Modules.Length.ShouldBe(2);
            model.Environments.Count.ShouldBe(2);
            model.MachineName.ShouldBe("prod-server-01");
            model.NumberOfProcessors.ShouldBe("8");
            model.TotalAvailableMemory.ShouldBe("16 GB");
            model.CurrentTimeZoneLocal.ShouldBe("America/Sao_Paulo");
            model.CurrentEnviromment.ShouldBe("Production");
            model.CurrentDirectory.ShouldBe("/app");
            model.PagedMemorySize.ShouldBe("100 MB");
            model.PrivateMemorySize.ShouldBe("200 MB");
            model.VirtualMemorySize.ShouldBe("500 MB");
            model.WorkingMemoryUsed.ShouldBe("150 MB");
            model.OSVersion.ShouldBe("Ubuntu 22.04");
        }
    }
}
