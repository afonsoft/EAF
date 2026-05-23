using Eaf.Hangfire;
using Shouldly;
using System;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Hangfire
{
    public class EafHangFireOptionsTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveDefinirPadroes()
        {
            var options = new EafHangFireOptions();

            options.PathMatch.ShouldBe("/hangfire");
            options.IsEnabled.ShouldBeTrue();
            options.StorageType.ShouldBe(HangfireStorageType.SqlServer);
            options.RequiredPermissionName.ShouldContain("Pages.Administration");
            options.RequiredPermissionName.ShouldContain("Pages.Administration.HangfireDashboard");
            options.Queues.ShouldContain("default");
            options.WorkerCount.ShouldBeGreaterThan(0);
            options.WorkerCount.ShouldBeLessThanOrEqualTo(16);
        }

        [Fact]
        public void Dado_IOptions_Quando_AcessarValue_Entao_DeveRetornarPropria()
        {
            var options = new EafHangFireOptions();
            options.Value.ShouldBe(options);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirPathMatch_Entao_DeveArmazenar()
        {
            var options = new EafHangFireOptions { PathMatch = "/jobs" };
            options.PathMatch.ShouldBe("/jobs");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirAppPath_Entao_DeveArmazenar()
        {
            var options = new EafHangFireOptions { AppPath = "/app" };
            options.AppPath.ShouldBe("/app");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirPrefixPath_Entao_DeveArmazenar()
        {
            var options = new EafHangFireOptions { PrefixPath = "/admin" };
            options.PrefixPath.ShouldBe("/admin");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDashboardTitle_Entao_DeveArmazenar()
        {
            var options = new EafHangFireOptions { DashboardTitle = "My Jobs" };
            options.DashboardTitle.ShouldBe("My Jobs");
        }

        [Fact]
        public void Dado_Instancia_Quando_DesabilitarIsEnabled_Entao_DeveSerFalse()
        {
            var options = new EafHangFireOptions { IsEnabled = false };
            options.IsEnabled.ShouldBeFalse();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirStorageTypeRedis_Entao_DeveArmazenar()
        {
            var options = new EafHangFireOptions { StorageType = HangfireStorageType.Redis };
            options.StorageType.ShouldBe(HangfireStorageType.Redis);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirStorageTypeInMemory_Entao_DeveArmazenar()
        {
            var options = new EafHangFireOptions { StorageType = HangfireStorageType.InMemory };
            options.StorageType.ShouldBe(HangfireStorageType.InMemory);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirStorageTypeSqlServer_Entao_DeveArmazenar()
        {
            var options = new EafHangFireOptions { StorageType = HangfireStorageType.SqlServer };
            options.StorageType.ShouldBe(HangfireStorageType.SqlServer);
        }
    }
}
