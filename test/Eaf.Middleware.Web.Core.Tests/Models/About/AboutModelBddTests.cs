using Eaf.Models.About;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Models.About
{
    /// <summary>
    /// Testes BDD para AboutModel seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AboutModelBddTests
    {
        [Fact]
        public void Dado_AboutModel_Quando_DefinirTodasPropriedades_Entao_DeveArmazenarCorretamente()
        {
            // Dado & Quando
            var model = new AboutModel
            {
                Version = "10.0.0",
                OSVersion = "Ubuntu 22.04",
                OS = "Linux",
                NumberOfProcessors = "8",
                MachineName = "srv-prod-01",
                Architecture = "x64",
                RuntimeIdentifier = "linux-x64",
                FrameworkDescription = ".NET 10.0.0",
                TotalAvailableMemory = "16 GB",
                CurrentCulture = "pt-BR",
                CurrentTimeZoneLocal = "America/Sao_Paulo",
                CurrentEnviromment = "Production",
                CurrentDirectory = "/app",
                ProcessName = "dotnet",
                PagedMemorySize = "256 MB",
                PrivateMemorySize = "512 MB",
                VirtualMemorySize = "2 GB",
                WorkingMemoryUsed = "384 MB",
                Modules = new[] { "Module1.dll", "Module2.dll" },
                Environments = new Dictionary<string, string>
                {
                    { "ASPNETCORE_ENVIRONMENT", "Production" },
                    { "DOTNET_RUNNING_IN_CONTAINER", "true" }
                }
            };

            // Então
            model.Version.ShouldBe("10.0.0");
            model.OSVersion.ShouldBe("Ubuntu 22.04");
            model.OS.ShouldBe("Linux");
            model.NumberOfProcessors.ShouldBe("8");
            model.MachineName.ShouldBe("srv-prod-01");
            model.Architecture.ShouldBe("x64");
            model.RuntimeIdentifier.ShouldBe("linux-x64");
            model.FrameworkDescription.ShouldBe(".NET 10.0.0");
            model.TotalAvailableMemory.ShouldBe("16 GB");
            model.CurrentCulture.ShouldBe("pt-BR");
            model.CurrentTimeZoneLocal.ShouldBe("America/Sao_Paulo");
            model.CurrentEnviromment.ShouldBe("Production");
            model.CurrentDirectory.ShouldBe("/app");
            model.ProcessName.ShouldBe("dotnet");
            model.PagedMemorySize.ShouldBe("256 MB");
            model.PrivateMemorySize.ShouldBe("512 MB");
            model.VirtualMemorySize.ShouldBe("2 GB");
            model.WorkingMemoryUsed.ShouldBe("384 MB");
            model.Modules.Length.ShouldBe(2);
            model.Environments.Count.ShouldBe(2);
        }
    }
}
