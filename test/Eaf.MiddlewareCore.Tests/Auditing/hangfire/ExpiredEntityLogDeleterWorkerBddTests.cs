using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.EntityHistory;
using Abp.Hangfire;
using Eaf.Middleware.Configuration;
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
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Auditing.hangfire
{
    public class ExpiredEntityLogDeleterWorkerBddTests
    {
        private readonly IRepository<EntityChange, long> _entityChangeRepository;
        private readonly IEntityHistoryConfiguration _entityHistoryConfiguration;
        private readonly ISettingManager _settingManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IActiveUnitOfWork _activeUnitOfWork;

        public ExpiredEntityLogDeleterWorkerBddTests()
        {
            _entityChangeRepository = Substitute.For<IRepository<EntityChange, long>>();
            _entityHistoryConfiguration = Substitute.For<IEntityHistoryConfiguration>();
            _settingManager = Substitute.For<ISettingManager>();
            _activeUnitOfWork = Substitute.For<IActiveUnitOfWork>();
            _activeUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            _activeUnitOfWork.DisableFilter(Arg.Any<string[]>()).Returns(Substitute.For<IDisposable>());

            _unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            _unitOfWorkManager.Current.Returns(_activeUnitOfWork);
        }

        private TestableExpiredEntityLogDeleterWorker CreateWorker(bool isEnabled)
        {
            _entityHistoryConfiguration.IsEnabled.Returns(isEnabled);
            return new TestableExpiredEntityLogDeleterWorker(_entityChangeRepository, _entityHistoryConfiguration, _settingManager)
            {
                UnitOfWorkManager = _unitOfWorkManager
            };
        }

        private sealed class TestableExpiredEntityLogDeleterWorker : ExpiredEntityLogDeleterWorker
        {
            public TestableExpiredEntityLogDeleterWorker(
                IRepository<EntityChange, long> auditLogRepository,
                IEntityHistoryConfiguration historyConfiguration,
                ISettingManager settingManager)
                : base(auditLogRepository, historyConfiguration, settingManager)
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
            var sut = CreateWorker(true);

            sut.ShouldNotBeNull();
            sut.ShouldBeAssignableTo<DomainService>();
        }

        [Fact]
        public void Dado_EntityHistoryDesabilitado_Quando_DoWork_Entao_DeveRetornarSemDeletar()
        {
            _entityChangeRepository.LongCount(Arg.Any<Expression<Func<EntityChange, bool>>>()).Returns(5L);
            var sut = CreateWorker(false);
            var context = CriarPerformContext();

            sut.DoWorkPublic(context);

            _entityChangeRepository.DidNotReceive().Delete(Arg.Any<EntityChange>());
        }

        [Fact]
        public void Dado_EntityHistoryHabilitado_Quando_DoWork_Entao_DeveDeletarEntityChanges()
        {
            var entityChanges = new List<EntityChange>
            {
                new EntityChange { Id = 1, ChangeTime = DateTime.UtcNow.AddDays(-200) }
            };

            _entityChangeRepository.GetAll().Returns(entityChanges.AsQueryable());
            _entityChangeRepository.LongCount(Arg.Any<Expression<Func<EntityChange, bool>>>()).Returns(1L);
            _entityChangeRepository.When(x => x.Delete(entityChanges[0])).Do(_ => { });

            var sut = CreateWorker(true);
            var context = CriarPerformContext();

            sut.DoWorkPublic(context);

            _entityChangeRepository.Received(1).Delete(entityChanges[0]);
        }

        [Fact]
        public void Dado_NenhumEntityChangeExpirado_Quando_DoWork_Entao_NaoDeveDeletar()
        {
            _entityChangeRepository.LongCount(Arg.Any<Expression<Func<EntityChange, bool>>>()).Returns(0L);
            var sut = CreateWorker(true);
            var context = CriarPerformContext();

            sut.DoWorkPublic(context);

            _entityChangeRepository.DidNotReceive().Delete(Arg.Any<EntityChange>());
        }

        [Fact]
        public void Dado_SettingsInvalidos_Quando_DoWork_Entao_DeveUsarValoresPadrao()
        {
            _settingManager.GetSettingValue(Arg.Any<string>()).Returns((string)null);
            _entityChangeRepository.LongCount(Arg.Any<Expression<Func<EntityChange, bool>>>()).Returns(0L);
            var sut = CreateWorker(true);
            var context = CriarPerformContext();

            sut.DoWorkPublic(context);

            _entityChangeRepository.DidNotReceive().Delete(Arg.Any<EntityChange>());
        }
    }
}
