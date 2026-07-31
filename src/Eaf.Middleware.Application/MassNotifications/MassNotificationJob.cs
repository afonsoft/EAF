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

            if (massNotification.ScheduledTime.HasValue && massNotification.ScheduledTime.Value > Clock.Now)
            {
                var delay = massNotification.ScheduledTime.Value - Clock.Now;
                if (delay > TimeSpan.Zero)
                {
                    await _backgroundJobManager.EnqueueAsync<MassNotificationJob, MassNotificationJobArgs>(
                        args,
                        delay: delay);
                }
                return;
            }

            var userIds = new HashSet<long>();

            if (!string.IsNullOrWhiteSpace(massNotification.TargetUserIds))
            {
                foreach (var id in ParseLongIds(massNotification.TargetUserIds))
                {
                    userIds.Add(id);
                }
            }

            if (!string.IsNullOrWhiteSpace(massNotification.TargetOrganizationUnitIds))
            {
                foreach (var ouId in ParseLongIds(massNotification.TargetOrganizationUnitIds))
                {
                    var users = await _userManager.GetUsersInOrganizationUnitAsync(new OrganizationUnit { Id = ouId }, false);
                    foreach (var user in users)
                    {
                        userIds.Add(user.Id);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(massNotification.TargetRoleIds))
            {
                foreach (var roleId in ParseIntIds(massNotification.TargetRoleIds))
                {
                    var role = await _roleManager.GetRoleByIdAsync(roleId);
                    var users = await _userManager.GetUsersInRoleAsync(role.Name);
                    foreach (var user in users)
                    {
                        userIds.Add(user.Id);
                    }
                }
            }

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

            massNotification.Status = MassNotificationStatus.Sent;
            await _massNotificationRepository.UpdateAsync(massNotification);
        }

        private static IEnumerable<long> ParseLongIds(string value)
        {
            return value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => long.TryParse(v.Trim(), out var id) ? id : (long?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value);
        }

        private static IEnumerable<int> ParseIntIds(string value)
        {
            return value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => int.TryParse(v.Trim(), out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value);
        }
    }
}
