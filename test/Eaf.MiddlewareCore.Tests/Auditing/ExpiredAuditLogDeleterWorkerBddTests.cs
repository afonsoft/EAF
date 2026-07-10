using Abp.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.Timers;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Web.Auditing;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Auditing
{
    public class ExpiredAuditLogDeleterWorkerBddTests
    {
        private readonly IRepository<AuditLog, long> _auditLogRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IActiveUnitOfWork _activeUnitOfWork;
        private readonly IUnitOfWorkCompleteHandle _unitOfWorkHandle;

        public ExpiredAuditLogDeleterWorkerBddTests()
        {
            _auditLogRepository = Substitute.For<IRepository<AuditLog, long>>();
            _tenantRepository = Substitute.For<IRepository<Tenant>>();
            _activeUnitOfWork = Substitute.For<IActiveUnitOfWork>();
            _activeUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            _activeUnitOfWork.DisableFilter(Arg.Any<string[]>()).Returns(Substitute.For<IDisposable>());

            _unitOfWorkHandle = Substitute.For<IUnitOfWorkCompleteHandle>();
            _unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            _unitOfWorkManager.Begin().Returns(_unitOfWorkHandle);
            _unitOfWorkManager.Current.Returns(_activeUnitOfWork);
        }

        private TestableExpiredAuditLogDeleterWorker CreateWorker()
        {
            return new TestableExpiredAuditLogDeleterWorker(
                new AbpTimer(),
                _auditLogRepository,
                _tenantRepository)
            {
                UnitOfWorkManager = _unitOfWorkManager
            };
        }

        private sealed class TestableExpiredAuditLogDeleterWorker : ExpiredAuditLogDeleterWorker
        {
            public TestableExpiredAuditLogDeleterWorker(
                AbpTimer timer,
                IRepository<AuditLog, long> auditLogRepository,
                IRepository<Tenant> tenantRepository)
                : base(timer, auditLogRepository, tenantRepository)
            {
            }

            public void DoWorkPublic() => DoWork();
        }

        [Fact]
        public void Dado_Timer_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            // Dado & Quando
            var sut = CreateWorker();

            // Então
            sut.ShouldNotBeNull();
            sut.ShouldBeAssignableTo<Abp.Threading.BackgroundWorkers.PeriodicBackgroundWorkerBase>();
        }

        [Fact]
        public void Dado_SemTenants_Quando_DoWork_Entao_DeveChamarDeleteAuditLogsOnHost()
        {
            // Dado
            var expireDate = DateTime.UtcNow.AddDays(-400);
            _tenantRepository.GetAll().Returns(new List<Tenant>().AsQueryable());
            _auditLogRepository.LongCount(Arg.Any<Expression<Func<AuditLog, bool>>>()).Returns(0L);

            var sut = CreateWorker();

            // Quando
            sut.DoWorkPublic();

            // Então
            _unitOfWorkManager.Received(2).Begin();
            _auditLogRepository.Received(1).LongCount(Arg.Any<Expression<Func<AuditLog, bool>>>());
        }

        [Fact]
        public void Dado_TenantComConnectionString_Quando_DoWork_Entao_DeveChamarDeleteAuditLogsTenant()
        {
            // Dado
            var tenant = new Tenant("tenant1", "Tenant 1") { ConnectionString = "conn" };
            _tenantRepository.GetAll().Returns(new List<Tenant> { tenant }.AsQueryable());
            _auditLogRepository.LongCount(Arg.Any<Expression<Func<AuditLog, bool>>>()).Returns(0L);

            var sut = CreateWorker();

            // Quando
            sut.DoWorkPublic();

            // Então
            _unitOfWorkManager.Received(3).Begin();
        }

        [Fact]
        public void Dado_AuditLogsExpirados_Quando_DoWork_Entao_DeveDeletarPelaData()
        {
            // Dado
            var tenant = new Tenant("tenant1", "Tenant 1") { ConnectionString = "conn" };
            _tenantRepository.GetAll().Returns(new List<Tenant> { tenant }.AsQueryable());
            _auditLogRepository.LongCount(Arg.Any<Expression<Func<AuditLog, bool>>>()).Returns(1L);

            var sut = CreateWorker();

            // Quando
            sut.DoWorkPublic();

            // Então
            _auditLogRepository.Received(2).Delete(Arg.Any<Expression<Func<AuditLog, bool>>>());
        }

        [Fact]
        public void Dado_ExcessoDeAuditLogs_Quando_DoWork_Entao_DeveDeletarPorIdsLimitados()
        {
            // Dado
            var auditLogs = Enumerable.Range(1, 30005)
                .Select(i => new AuditLog { Id = i, ExecutionTime = DateTime.UtcNow.AddDays(-400) })
                .ToList();

            _tenantRepository.GetAll().Returns(new List<Tenant>().AsQueryable());
            _auditLogRepository.GetAll().Returns(auditLogs.AsQueryable());
            _auditLogRepository.LongCount(Arg.Any<Expression<Func<AuditLog, bool>>>()).Returns(40000L);

            var sut = CreateWorker();

            // Quando
            sut.DoWorkPublic();

            // Então
            _auditLogRepository.Received(1).Delete(Arg.Any<Expression<Func<AuditLog, bool>>>());
        }

        [Fact]
        public void Dado_ErroNoUowDoHost_Quando_DoWork_Entao_NaoDeveLancarExcecao()
        {
            // Dado
            _tenantRepository.GetAll().Returns(new List<Tenant>().AsQueryable());
            _unitOfWorkManager.Begin().Returns(_ => _unitOfWorkHandle, _ => throw new Exception("UOW error"));

            var sut = CreateWorker();

            // Quando & Então
            Should.NotThrow(() => sut.DoWorkPublic());
        }
    }
}
