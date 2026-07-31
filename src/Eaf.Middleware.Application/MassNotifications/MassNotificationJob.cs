using Abp;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Notifications;
using Abp.Organizations;
using Abp.Timing;
using Eaf.BackgroundJobs;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Hangfire.Server;
using IBackgroundJobManager = Abp.BackgroundJobs.IBackgroundJobManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.Middleware.MassNotifications
{
    /// <summary>
    /// Background job que envia uma notificação em massa.
    /// </summary>
    public class MassNotificationJob : AsyncBackgroundJob<MassNotificationJobArgs>
    {
        private readonly IRepository<MassNotification, long> _massNotificationRepository;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;

        /// <summary>
        /// MassNotificationJob.
        /// </summary>
        public MassNotificationJob(
            IRepository<MassNotification, long> massNotificationRepository,
            INotificationPublisher notificationPublisher,
            IBackgroundJobManager backgroundJobManager,
            UserManager userManager,
            RoleManager roleManager)
        {
            _massNotificationRepository = massNotificationRepository;
            _notificationPublisher = notificationPublisher;
            _backgroundJobManager = backgroundJobManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Executa o envio da notificação em massa.
        /// </summary>
        public override async Task ExecuteAsync(MassNotificationJobArgs args, PerformContext context, CancellationToken token)
        {
            var massNotification = await _massNotificationRepository.GetAsync(args.MassNotificationId);
            if (massNotification == null || massNotification.Status == MassNotificationStatus.Canceled)
                return;

            if (await TryRescheduleAsync(massNotification, args))
                return;

            var userIds = await CollectTargetUserIdsAsync(massNotification);
            await PublishAsync(massNotification, userIds);

            massNotification.Status = MassNotificationStatus.Sent;
            await _massNotificationRepository.UpdateAsync(massNotification);
        }

        private async Task<bool> TryRescheduleAsync(MassNotification massNotification, MassNotificationJobArgs args)
        {
            if (!massNotification.ScheduledTime.HasValue || massNotification.ScheduledTime.Value <= Clock.Now)
                return false;

            var delay = massNotification.ScheduledTime.Value - Clock.Now;
            if (delay <= TimeSpan.Zero)
                return false;

            await _backgroundJobManager.EnqueueAsync<MassNotificationJob, MassNotificationJobArgs>(args, delay: delay);
            return true;
        }

        private async Task<HashSet<long>> CollectTargetUserIdsAsync(MassNotification massNotification)
        {
            var userIds = new HashSet<long>();

            await AddUserIdsAsync(massNotification.TargetUserIds, userIds, AddDirectUserIds);
            await AddUserIdsAsync(massNotification.TargetOrganizationUnitIds, userIds, AddOrganizationUnitUserIds);
            await AddUserIdsAsync(massNotification.TargetRoleIds, userIds, AddRoleUserIds);

            return userIds;
        }

        private static async Task AddUserIdsAsync(string ids, HashSet<long> userIds, Func<string, HashSet<long>, Task> addAction)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return;

            await addAction(ids, userIds);
        }

        private static Task AddDirectUserIds(string ids, HashSet<long> userIds)
        {
            foreach (var id in ParseLongIds(ids))
                userIds.Add(id);

            return Task.CompletedTask;
        }

        private async Task AddOrganizationUnitUserIds(string ids, HashSet<long> userIds)
        {
            foreach (var ouId in ParseLongIds(ids))
            {
                var users = await _userManager.GetUsersInOrganizationUnitAsync(new OrganizationUnit { Id = ouId }, false);
                foreach (var user in users)
                    userIds.Add(user.Id);
            }
        }

        private async Task AddRoleUserIds(string ids, HashSet<long> userIds)
        {
            foreach (var roleId in ParseIntIds(ids))
            {
                var role = await _roleManager.GetRoleByIdAsync(roleId);
                var users = await _userManager.GetUsersInRoleAsync(role.Name);
                foreach (var user in users)
                    userIds.Add(user.Id);
            }
        }

        private async Task PublishAsync(MassNotification massNotification, HashSet<long> userIds)
        {
            var identifiers = userIds.Select(id => new UserIdentifier(massNotification.TenantId, id)).ToArray();

            var data = new MassNotificationData
            {
                Subject = massNotification.Subject,
                Message = massNotification.Message,
            };

            int?[] tenantIds = massNotification.SendToAllUsers && massNotification.TenantId.HasValue
                ? new int?[] { massNotification.TenantId.Value }
                : null;

            if (identifiers.Any() || tenantIds != null)
            {
                await _notificationPublisher.PublishAsync(
                    "MassNotification",
                    data,
                    severity: massNotification.Severity,
                    userIds: identifiers,
                    tenantIds: tenantIds);
            }
        }

        private static IEnumerable<long> ParseLongIds(string value)
        {
            return value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => long.TryParse(v.Trim(), out var id) ? (long?)id : null)
                .OfType<long>();
        }

        private static IEnumerable<int> ParseIntIds(string value)
        {
            return value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => int.TryParse(v.Trim(), out var id) ? (int?)id : null)
                .OfType<int>();
        }
    }
}
