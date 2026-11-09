using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Notifications;
using Abp.Threading;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eaf.Workflow.Domain
{
    /// <summary>
    /// SLA reminder worker: scans pending workflow tasks past their due time and notifies
    /// assignees once per task (idempotent via "sla-reminded" timeline entries).
    /// </summary>
    public class WorkflowSlaWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private const int CheckPeriodAsMilliseconds = 1000 * 60 * 60; // 1 hora

        /// <summary>Notification name used for overdue workflow tasks.</summary>
        public const string TaskOverdueNotificationName = "Eaf.Workflow.TaskOverdue";

        private readonly IRepository<WorkflowTask, Guid> _tasks;
        private readonly IRepository<WorkflowTimelineEntry, long> _timeline;
        private readonly INotificationPublisher _notificationPublisher;

        /// <summary>WorkflowSlaWorker.</summary>
        public WorkflowSlaWorker(
            AbpTimer timer,
            IRepository<WorkflowTask, Guid> tasks,
            IRepository<WorkflowTimelineEntry, long> timeline,
            INotificationPublisher notificationPublisher)
            : base(timer)
        {
            _tasks = tasks;
            _timeline = timeline;
            _notificationPublisher = notificationPublisher;

            Timer.Period = CheckPeriodAsMilliseconds;
            Timer.RunOnStart = false;
        }

        /// <summary>
        /// Verifica tarefas vencidas e notifica os responsáveis.
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
                var now = Clock.Now;
                var overdue = await _tasks.GetAllListAsync(t =>
                    t.Status == WorkflowTaskStatus.Pending &&
                    t.DueTime.HasValue &&
                    t.DueTime.Value <= now);
                if (overdue.Count == 0)
                {
                    return;
                }

                var instanceIds = overdue.Select(t => t.InstanceId).Distinct().ToList();
                var reminders = await _timeline.GetAllListAsync(e =>
                    e.EventType == "sla-reminded" && instanceIds.Contains(e.InstanceId));
                var reminded = new HashSet<Guid>(reminders
                    .Select(e => ParseTaskId(e.DataJson))
                    .Where(id => id.HasValue)
                    .Select(id => id.Value));

                foreach (var task in overdue.Where(t => !reminded.Contains(t.Id)))
                {
                    var data = new NotificationData();
                    data.Properties["message"] = $"Workflow task '{task.StepId}' is overdue.";
                    data.Properties["taskId"] = task.Id.ToString();
                    data.Properties["instanceId"] = task.InstanceId.ToString();

                    await _notificationPublisher.PublishAsync(
                        TaskOverdueNotificationName,
                        data,
                        userIds: new[] { new UserIdentifier(task.TenantId, task.AssigneeUserId) });

                    await _timeline.InsertAsync(new WorkflowTimelineEntry
                    {
                        InstanceId = task.InstanceId,
                        EventType = "sla-reminded",
                        StepId = task.StepId,
                        ActorUserId = task.AssigneeUserId,
                        TenantId = task.TenantId,
                        DataJson = JsonSerializer.Serialize(new { taskId = task.Id })
                    });
                }

                await uow.CompleteAsync();
            }
        }

        private static Guid? ParseTaskId(string dataJson)
        {
            if (string.IsNullOrEmpty(dataJson))
            {
                return null;
            }
            try
            {
                using var doc = JsonDocument.Parse(dataJson);
                if (doc.RootElement.TryGetProperty("taskId", out var el) &&
                    Guid.TryParse(el.GetString(), out var id))
                {
                    return id;
                }
            }
            catch (JsonException)
            {
                // ignore malformed payloads
            }
            return null;
        }
    }
}
