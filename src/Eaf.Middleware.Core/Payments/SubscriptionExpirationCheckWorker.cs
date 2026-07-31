using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Notifications;
using Abp.Threading;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using Eaf.Middleware.MultiTenancy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Worker que verifica periodicamente assinaturas expiradas e notifica os tenants afetados.
    /// </summary>
    public class SubscriptionExpirationCheckWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1000 * 60 * 60 * 24; // 1 dia

        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly INotificationPublisher _notificationPublisher;

        /// <summary>
        /// SubscriptionExpirationCheckWorker.
        /// </summary>
        public SubscriptionExpirationCheckWorker(
            AbpTimer timer,
            IRepository<Tenant, int> tenantRepository,
            INotificationPublisher notificationPublisher)
            : base(timer)
        {
            _tenantRepository = tenantRepository;
            _notificationPublisher = notificationPublisher;

            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = true;
        }

        /// <summary>
        /// Verifica expirações e notifica os tenants.
        /// </summary>
        protected override void DoWork()
        {
            AsyncHelper.RunSync(DoWorkAsync);
        }

        private async Task DoWorkAsync()
        {
            using var uow = UnitOfWorkManager.Begin();
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var today = Clock.Now.Date;
                var tenants = await _tenantRepository.GetAllListAsync(t =>
                    t.IsActive &&
                    t.EditionId.HasValue &&
                    t.SubscriptionEndDateUtc.HasValue &&
                    t.SubscriptionEndDateUtc.Value < today);

                foreach (var tenant in tenants)
                {
                    await NotifyTenantSubscriptionExpiredAsync(tenant);
                }

                uow.Complete();
            }
        }

        private async Task NotifyTenantSubscriptionExpiredAsync(Tenant tenant)
        {
            var data = new NotificationData();
            data.Properties["tenantId"] = tenant.Id;
            data.Properties["tenantName"] = tenant.Name;

            await _notificationPublisher.PublishAsync(
                "SubscriptionExpired",
                data,
                tenantIds: new int?[] { tenant.Id });
        }
    }
}
