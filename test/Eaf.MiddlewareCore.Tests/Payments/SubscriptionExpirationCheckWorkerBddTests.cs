using Abp;
using Abp.Domain.Repositories;
using Abp.Notifications;
using Abp.Threading.Timers;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Payments;
using Eaf.Middleware.Tests.Helpers;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Payments
{
    /// <summary>
    /// Testes BDD para SubscriptionExpirationCheckWorker seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class SubscriptionExpirationCheckWorkerBddTests
    {
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly TestableWorker _sut;

        public SubscriptionExpirationCheckWorkerBddTests()
        {
            _tenantRepository = Substitute.For<IRepository<Tenant, int>>();
            _notificationPublisher = Substitute.For<INotificationPublisher>();

            _sut = new TestableWorker(new AbpTimer(), _tenantRepository, _notificationPublisher)
            {
                UnitOfWorkManager = CoreManagerTestHelper.CreateUnitOfWorkManager(),
            };
        }

        private sealed class TestableWorker : SubscriptionExpirationCheckWorker
        {
            public TestableWorker(
                AbpTimer timer,
                IRepository<Tenant, int> tenantRepository,
                INotificationPublisher notificationPublisher)
                : base(timer, tenantRepository, notificationPublisher)
            {
            }

            public void ExecutarTrabalho() => DoWork();

            public AbpTimer ObterTimer() => Timer;
        }

        [Fact]
        public void Dado_Worker_Quando_Construir_Entao_DeveConfigurarTimerDiario()
        {
            // Então
            _sut.ObterTimer().Period.ShouldBe(1000 * 60 * 60 * 24);
            _sut.ObterTimer().RunOnStart.ShouldBeTrue();
        }

        [Fact]
        public void Dado_TenantsExpirados_Quando_Executar_Entao_DeveNotificarCadaTenant()
        {
            // Dado
            var tenants = new List<Tenant>
            {
                new Tenant("tenant1", "Tenant Um") { Id = 1 },
                new Tenant("tenant2", "Tenant Dois") { Id = 2 },
            };
            _tenantRepository.GetAllListAsync(Arg.Any<Expression<Func<Tenant, bool>>>()).Returns(tenants);

            // Quando
            _sut.ExecutarTrabalho();

            // Então
            _notificationPublisher.Received(2).PublishAsync(
                "SubscriptionExpired",
                Arg.Any<NotificationData>(),
                Arg.Any<Abp.Domain.Entities.EntityIdentifier>(),
                Arg.Any<NotificationSeverity>(),
                Arg.Any<UserIdentifier[]>(),
                Arg.Any<UserIdentifier[]>(),
                Arg.Is<int?[]>(t => t.Length == 1),
                Arg.Any<Type[]>());
        }

        [Fact]
        public void Dado_SemTenantsExpirados_Quando_Executar_Entao_NaoDeveNotificar()
        {
            // Dado
            _tenantRepository.GetAllListAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
                .Returns(new List<Tenant>());

            // Quando
            _sut.ExecutarTrabalho();

            // Então
            _notificationPublisher.DidNotReceive().PublishAsync(
                Arg.Any<string>(),
                Arg.Any<NotificationData>(),
                Arg.Any<Abp.Domain.Entities.EntityIdentifier>(),
                Arg.Any<NotificationSeverity>(),
                Arg.Any<UserIdentifier[]>(),
                Arg.Any<UserIdentifier[]>(),
                Arg.Any<int?[]>(),
                Arg.Any<Type[]>());
        }
    }
}
