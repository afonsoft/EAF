using Abp.Auditing;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.Configuration;
using Abp.Threading.BackgroundWorkers;
using Eaf.Auditing.hangfire;
using NSubstitute;
using Shouldly;
using Xunit;
using PeriodicAuditWorker = Eaf.Middleware.Web.Auditing.ExpiredAuditLogDeleterWorker;
using HangfireAuditWorker = Eaf.Middleware.Web.Auditing.hangfire.ExpiredAuditLogDeleterWorker;
using HangfireEntityWorker = Eaf.Middleware.Web.Auditing.hangfire.ExpiredEntityLogDeleterWorker;

namespace Eaf.Middleware.Tests.Auditing
{
    /// <summary>
    /// Testes BDD para os workers de exclusão de logs expirados seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class ExpiredLogDeleterWorkersBddTests
    {
        [Fact]
        public void Dado_WorkerPeriodico_Quando_VerificarIsEnabled_Entao_DeveSerTrue()
        {
            PeriodicAuditWorker.IsEnabled.ShouldBeTrue();
        }

        [Fact]
        public void Dado_WorkerPeriodico_Quando_VerificarTipo_Entao_DeveHerdarPeriodicBackgroundWorkerBase()
        {
            typeof(PeriodicBackgroundWorkerBase).IsAssignableFrom(typeof(PeriodicAuditWorker)).ShouldBeTrue();
            typeof(ISingletonDependency).IsAssignableFrom(typeof(PeriodicAuditWorker)).ShouldBeTrue();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarHangfireAuditWorker_Entao_DeveInstanciar()
        {
            var repository = Substitute.For<IRepository<AuditLog, long>>();
            var auditingConfiguration = Substitute.For<IAuditingConfiguration>();

            var worker = new HangfireAuditWorker(repository, auditingConfiguration);

            worker.ShouldNotBeNull();
            worker.ShouldBeAssignableTo<IExpiredAuditLogDeleterWorker>();
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarHangfireEntityWorker_Entao_DeveInstanciar()
        {
            var repository = Substitute.For<IRepository<EntityChange, long>>();
            var historyConfiguration = Substitute.For<IEntityHistoryConfiguration>();
            var settingManager = Substitute.For<ISettingManager>();

            var worker = new HangfireEntityWorker(repository, historyConfiguration, settingManager);

            worker.ShouldNotBeNull();
            worker.ShouldBeAssignableTo<IExpiredEntityLogDeleterWorker>();
        }
    }
}
