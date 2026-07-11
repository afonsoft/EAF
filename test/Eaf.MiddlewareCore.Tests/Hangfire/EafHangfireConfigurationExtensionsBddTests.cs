using Abp.Auditing;
using Abp.BackgroundJobs;
using Abp.Configuration.Startup;
using Abp.EntityHistory;
using Eaf.Auditing.hangfire;
using Eaf.Hangfire;
using Hangfire;
using Hangfire.MemoryStorage;
using NSubstitute;
using Shouldly;
using System;
using System.Reflection;
using Xunit;

namespace Eaf.Hangfire.Tests
{
    public class EafHangfireConfigurationExtensionsBddTests : IDisposable
    {
        private readonly JobStorage _originalJobStorage;

        public EafHangfireConfigurationExtensionsBddTests()
        {
            var currentField = typeof(JobStorage).GetField("_current", BindingFlags.NonPublic | BindingFlags.Static);
            _originalJobStorage = currentField?.GetValue(null) as JobStorage ?? new MemoryStorage();
            JobStorage.Current = new MemoryStorage();
        }

        public void Dispose()
        {
            JobStorage.Current = _originalJobStorage ?? new MemoryStorage();
        }

        [Fact]
        public void Dado_BackgroundJobConfiguration_Quando_UseHangfire_Entao_DeveHabilitarExecucaoERegistrarServico()
        {
            var configuration = Substitute.For<IAbpStartupConfiguration>();
            var backgroundJobConfiguration = Substitute.For<IBackgroundJobConfiguration>();
            backgroundJobConfiguration.AbpConfiguration.Returns(configuration);

            backgroundJobConfiguration.UseHangfire();

            backgroundJobConfiguration.IsJobExecutionEnabled.ShouldBeTrue();
            configuration.Received(1).ReplaceService(typeof(IBackgroundJobManager), Arg.Any<Action>());
        }

        [Fact]
        public void Dado_AuditingHabilitado_Quando_SetExpiredAuditWoker_Entao_DeveLerIsEnabledEAgendarJob()
        {
            var auditingConfiguration = Substitute.For<IAuditingConfiguration>();
            auditingConfiguration.IsEnabled.Returns(true);

            Should.NotThrow(() => auditingConfiguration.SetExpiredAuditWoker());

            _ = auditingConfiguration.Received(1).IsEnabled;
            auditingConfiguration.IsEnabled.ShouldBeTrue();
        }

        [Fact]
        public void Dado_AuditingDesabilitado_Quando_SetExpiredAuditWoker_Entao_NaoDeveAgendarJob()
        {
            var auditingConfiguration = Substitute.For<IAuditingConfiguration>();
            auditingConfiguration.IsEnabled.Returns(false);

            Should.NotThrow(() => auditingConfiguration.SetExpiredAuditWoker());

            _ = auditingConfiguration.Received(1).IsEnabled;
            auditingConfiguration.IsEnabled.ShouldBeFalse();
        }

        [Fact]
        public void Dado_EntityHistoryHabilitado_Quando_SetExpiredHistoryEntityWoker_Entao_DeveLerIsEnabledEAgendarJob()
        {
            var entityHistoryConfiguration = Substitute.For<IEntityHistoryConfiguration>();
            entityHistoryConfiguration.IsEnabled.Returns(true);

            Should.NotThrow(() => entityHistoryConfiguration.SetExpiredHistoryEntityWoker());

            _ = entityHistoryConfiguration.Received(1).IsEnabled;
            entityHistoryConfiguration.IsEnabled.ShouldBeTrue();
        }

        [Fact]
        public void Dado_EntityHistoryDesabilitado_Quando_SetExpiredHistoryEntityWoker_Entao_NaoDeveAgendarJob()
        {
            var entityHistoryConfiguration = Substitute.For<IEntityHistoryConfiguration>();
            entityHistoryConfiguration.IsEnabled.Returns(false);

            Should.NotThrow(() => entityHistoryConfiguration.SetExpiredHistoryEntityWoker());

            _ = entityHistoryConfiguration.Received(1).IsEnabled;
            entityHistoryConfiguration.IsEnabled.ShouldBeFalse();
        }
    }
}
