using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Notifications;
using Abp.Threading;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using Eaf.Middleware.Core.Editions;
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

        private static readonly int[] ReminderDays = { 7, 3, 1 };

        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IRepository<SubscriptionPayment, long> _subscriptionPaymentRepository;
        private readonly INotificationPublisher _notificationPublisher;

        /// <summary>
        /// SubscriptionExpirationCheckWorker.
        /// </summary>
        public SubscriptionExpirationCheckWorker(
            AbpTimer timer,
            IRepository<Tenant, int> tenantRepository,
            IRepository<SubscriptionPayment, long> subscriptionPaymentRepository,
            INotificationPublisher notificationPublisher)
            : base(timer)
        {
            _tenantRepository = tenantRepository;
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
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

                var expired = await _tenantRepository.GetAllListAsync(t =>
                    t.IsActive &&
                    t.EditionId.HasValue &&
                    t.SubscriptionEndDateUtc.HasValue &&
                    t.SubscriptionEndDateUtc.Value < today);

                foreach (var tenant in expired)
                {
                    await NotifyTenantSubscriptionExpiredAsync(tenant);
                }

                await NotifyTenantsExpiringSoonAsync(today);
                await ApplyPendingDowngradesAsync(today);

                await uow.CompleteAsync();
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

        private async Task NotifyTenantsExpiringSoonAsync(DateTime today)
        {
            foreach (var days in ReminderDays)
            {
                var target = today.AddDays(days);
                var expiring = await _tenantRepository.GetAllListAsync(t =>
                    t.IsActive &&
                    t.EditionId.HasValue &&
                    t.SubscriptionEndDateUtc.HasValue &&
                    t.SubscriptionEndDateUtc.Value.Date == target);

                foreach (var tenant in expiring)
                {
                    var data = new NotificationData();
                    data.Properties["tenantId"] = tenant.Id;
                    data.Properties["tenantName"] = tenant.Name;
                    data.Properties["daysLeft"] = days;

                    await _notificationPublisher.PublishAsync(
                        "SubscriptionExpiring",
                        data,
                        tenantIds: new int?[] { tenant.Id });
                }
            }
        }

        private async Task ApplyPendingDowngradesAsync(DateTime today)
        {
            var pending = await _subscriptionPaymentRepository.GetAllListAsync(p =>
                p.EditionPaymentType == EditionPaymentType.Downgrade &&
                p.IsProrationPayment &&
                p.Status == SubscriptionPaymentStatus.Completed &&
                p.TenantId.HasValue &&
                p.SubscriptionEndDate.HasValue &&
                p.SubscriptionEndDate.Value.Date < today);

            foreach (var payment in pending)
            {
                var tenant = await _tenantRepository.FirstOrDefaultAsync(payment.TenantId.Value);
                if (tenant == null || tenant.EditionId == payment.EditionId)
                {
                    continue;
                }

                tenant.EditionId = payment.EditionId;
                await _tenantRepository.UpdateAsync(tenant);
            }
        }
    }
}
