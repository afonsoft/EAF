using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Eaf.Middleware.Web.Auditing.hangfire;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Hangfire.Storage;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Auditing.hangfire
{
    public class ExpiredAuditLogDeleterWorkerHangfireBddTests
    {
        private readonly IRepository<AuditLog, long> _auditLogRepository;
        private readonly IAuditingConfiguration _auditingConfiguration;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IActiveUnitOfWork _activeUnitOfWork;

        public ExpiredAuditLogDeleterWorkerHangfireBddTests()
        {
            _auditLogRepository = Substitute.For<IRepository<AuditLog, long>>();
            _auditingConfiguration = Substitute.For<IAuditingConfiguration>();
            _activeUnitOfWork = Substitute.For<IActiveUnitOfWork>();
            _activeUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            _activeUnitOfWork.DisableFilter(Arg.Any<string[]>()).Returns(Substitute.For<IDisposable>());

            _unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            _unitOfWorkManager.Current.Returns(_activeUnitOfWork);
        }

        private TestableExpiredAuditLogDeleterWorker CreateWorker(bool isEnabled)
        {
            _auditingConfiguration.IsEnabled.Returns(isEnabled);
            return new TestableExpiredAuditLogDeleterWorker(_auditLogRepository, _auditingConfiguration)
            {
                UnitOfWorkManager = _unitOfWorkManager
            };
        }

        private sealed class TestableExpiredAuditLogDeleterWorker : ExpiredAuditLogDeleterWorker
        {
            public TestableExpiredAuditLogDeleterWorker(
                IRepository<AuditLog, long> auditLogRepository,
                IAuditingConfiguration historyConfiguration)
                : base(auditLogRepository, historyConfiguration)
            {
            }

            public void DoWorkPublic(PerformContext context) => DoWork(context);
        }

        private static PerformContext CriarPerformContext()
        {
            return new PerformContext(
                null,
                Substitute.For<IStorageConnection>(),
                new BackgroundJob("job-id", null, DateTime.UtcNow),
                Substitute.For<IJobCancellationToken>());
        }

        [Fact]
        public void Dado_Dependencias_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            // Dado & Quando
            var sut = CreateWorker(true);

            // Então
            sut.ShouldNotBeNull();
            sut.ShouldBeAssignableTo<DomainService>();
        }

        [Fact]
        public void Dado_AuditingDesabilitado_Quando_DoWork_Entao_DeveRetornarSemDeletar()
        {
            // Dado
            _auditLogRepository.LongCount(Arg.Any<Expression<Func<AuditLog, bool>>>()).Returns(5L);
            var sut = CreateWorker(false);
            var context = CriarPerformContext();

            // Quando
            sut.DoWorkPublic(context);

            // Então
            _auditLogRepository.DidNotReceive().Delete(Arg.Any<AuditLog>());
        }

        [Fact]
        public void Dado_AuditingHabilitado_Quando_DoWork_Entao_DeveDeletarAuditLogs()
        {
            // Dado
            var auditLogs = new List<AuditLog>
            {
                new AuditLog { Id = 1, ExecutionTime = DateTime.UtcNow.AddDays(-200) }
            };

            _auditLogRepository.GetAll().Returns(auditLogs.AsQueryable());
            _auditLogRepository.LongCount(Arg.Any<Expression<Func<AuditLog, bool>>>()).Returns(1L);
            _auditLogRepository.When(x => x.Delete(auditLogs[0])).Do(_ => { });

            var sut = CreateWorker(true);
            var context = CriarPerformContext();

            // Quando
            sut.DoWorkPublic(context);

            // Então
            _auditLogRepository.Received(1).Delete(auditLogs[0]);
        }

        [Fact]
        public void Dado_NenhumAuditLogExpirado_Quando_DoWork_Entao_NaoDeveDeletar()
        {
            // Dado
            _auditLogRepository.LongCount(Arg.Any<Expression<Func<AuditLog, bool>>>()).Returns(0L);
            var sut = CreateWorker(true);
            var context = CriarPerformContext();

            // Quando
            sut.DoWorkPublic(context);

            // Então
            _auditLogRepository.DidNotReceive().Delete(Arg.Any<AuditLog>());
        }
    }
}
