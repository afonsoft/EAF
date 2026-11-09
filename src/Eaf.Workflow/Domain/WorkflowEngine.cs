using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Timing;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eaf.Workflow.Domain
{
    /// <summary>
    /// Resolves assignee kinds (role, organization unit, manager) to user ids.
    /// Phase A ships a null implementation — the real directory is wired in Phase B.
    /// </summary>
    public interface IWorkflowUserDirectory
    {
        /// <summary>User ids in a role.</summary>
        Task<List<long>> GetUsersInRoleAsync(string roleName, int? tenantId);
        /// <summary>User ids in an organization unit.</summary>
        Task<List<long>> GetUsersInOrganizationUnitAsync(string ouCode, int? tenantId);
        /// <summary>Manager of a user (null when none).</summary>
        Task<long?> GetManagerAsync(long userId, int? tenantId);

        /// <summary>User ids that currently delegate to <paramref name="delegateUserId"/> (active delegations).</summary>
        Task<List<long>> GetActiveDelegatorIdsAsync(long delegateUserId, int? tenantId);
    }

    /// <summary>Default directory — resolves to empty sets (used until Phase B wires the real one).</summary>
    public class NullWorkflowUserDirectory : IWorkflowUserDirectory
    {
        /// <inheritdoc />
        public Task<List<long>> GetUsersInRoleAsync(string roleName, int? tenantId) => Task.FromResult(new List<long>());

        /// <inheritdoc />
        public Task<List<long>> GetUsersInOrganizationUnitAsync(string ouCode, int? tenantId) => Task.FromResult(new List<long>());

        /// <inheritdoc />
        public Task<long?> GetManagerAsync(long userId, int? tenantId) => Task.FromResult<long?>(null);

        /// <inheritdoc />
        public Task<List<long>> GetActiveDelegatorIdsAsync(long delegateUserId, int? tenantId) => Task.FromResult(new List<long>());
    }

    /// <summary>
    /// Core engine: start instances, activate steps, apply decisions, simulate paths.
    /// All transitions are idempotent at the task level — a decided task never fires twice.
    /// </summary>
    public interface IWorkflowEngine
    {
        /// <summary>Creates an instance bound to a published version and advances to the first approval.</summary>
        Task<WorkflowInstance> StartAsync(WorkflowVersion version, long requesterUserId, JsonElement formValues, int? tenantId);

        /// <summary>Applies an approve/reject/send-back decision to a pending task.</summary>
        Task<WorkflowInstance> DecideAsync(WorkflowTask task, long actorUserId, WorkflowTaskStatus decision, string comment);

        /// <summary>Walks the graph without persisting — returns the step ids a request would take and resolved assignees.</summary>
        Task<WorkflowSimulationResult> SimulateAsync(WorkflowVersion version, JsonElement formValues, long requesterUserId);

        /// <summary>Cancels a running instance (requester only).</summary>
        Task CancelAsync(WorkflowInstance instance, long actorUserId);
    }

    /// <summary>Simulation output.</summary>
    public class WorkflowSimulationResult
    {
        /// <summary>Step ids in execution order.</summary>
        public List<string> Path { get; set; } = new();

        /// <summary>StepId → resolved assignee user ids.</summary>
        public Dictionary<string, List<long>> Assignees { get; set; } = new();
    }

    /// <inheritdoc cref="IWorkflowEngine"/>
    public class WorkflowEngine : DomainService, IWorkflowEngine
    {
        private readonly IRepository<WorkflowInstance, Guid> _instances;
        private readonly IRepository<WorkflowTask, Guid> _tasks;
        private readonly IRepository<WorkflowTimelineEntry, long> _timeline;
        private readonly IRepository<WorkflowVersion, Guid> _versions;
        private readonly IWorkflowUserDirectory _userDirectory;
        private readonly WorkflowValidator _validator;

        /// <summary>WorkflowEngine.</summary>
        public WorkflowEngine(
            IRepository<WorkflowInstance, Guid> instances,
            IRepository<WorkflowTask, Guid> tasks,
            IRepository<WorkflowTimelineEntry, long> timeline,
            IRepository<WorkflowVersion, Guid> versions,
            IWorkflowUserDirectory userDirectory,
            WorkflowValidator validator)
        {
            _instances = instances;
            _tasks = tasks;
            _timeline = timeline;
            _versions = versions;
            _userDirectory = userDirectory;
            _validator = validator;
        }

        /// <inheritdoc />
        public async Task<WorkflowInstance> StartAsync(WorkflowVersion version, long requesterUserId, JsonElement formValues, int? tenantId)
        {
            var form = Deserialize<WorkflowFormModel>(version.FormJson);
            var missing = _validator.ValidateFormValues(form, formValues);
            if (missing.Count > 0)
            {
                throw new UserFriendlyException($"Required fields missing: {string.Join(", ", missing)}.");
            }

            var flow = Deserialize<WorkflowFlowModel>(version.FlowJson);
            var instance = new WorkflowInstance
            {
                Id = Guid.NewGuid(),
                DefinitionId = version.DefinitionId,
                Version = version.Version,
                RequesterUserId = requesterUserId,
                FormValuesJson = formValues.GetRawText(),
                Status = WorkflowInstanceStatus.InProgress,
                TenantId = tenantId
            };
            await _instances.InsertAsync(instance);
            await AddTimelineAsync(instance, "submitted", flow.StartStepId, requesterUserId, null);
            await AdvanceAsync(instance, flow, flow.StartStepId, requesterUserId);
            return instance;
        }

        /// <inheritdoc />
        public async Task<WorkflowInstance> DecideAsync(WorkflowTask task, long actorUserId, WorkflowTaskStatus decision, string comment)
        {
            if (task.Status != WorkflowTaskStatus.Pending)
            {
                throw new UserFriendlyException("This task was already decided.");
            }
            if (decision != WorkflowTaskStatus.Approved
                && decision != WorkflowTaskStatus.Rejected
                && decision != WorkflowTaskStatus.SentBack)
            {
                throw new UserFriendlyException("Invalid decision.");
            }

            var instance = await _instances.GetAsync(task.InstanceId);
            if (instance.Status != WorkflowInstanceStatus.InProgress || instance.CurrentStepId != task.StepId)
            {
                throw new UserFriendlyException("This task is no longer active.");
            }

            var versionFlow = await LoadFlowAsync(instance);
            var step = versionFlow.Steps.First(s => s.Id == task.StepId);

            if (decision == WorkflowTaskStatus.Rejected || decision == WorkflowTaskStatus.SentBack)
            {
                task.Status = decision;
                task.Comment = comment;
                await _tasks.UpdateAsync(task);
                await CancelPendingSiblingsAsync(instance, task.StepId, task.Id);

                var eventType = decision == WorkflowTaskStatus.Rejected ? "rejected" : "sent-back";
                await AddTimelineAsync(instance, eventType, task.StepId, actorUserId, comment);

                if (decision == WorkflowTaskStatus.SentBack)
                {
                    instance.Status = WorkflowInstanceStatus.SentBack;
                    instance.CompletedTime = Clock.Now;
                }
                else if (string.IsNullOrWhiteSpace(step.RejectStepId))
                {
                    instance.Status = WorkflowInstanceStatus.Rejected;
                    instance.CompletedTime = Clock.Now;
                }
                else
                {
                    await AdvanceAsync(instance, versionFlow, step.RejectStepId, actorUserId);
                }
                await _instances.UpdateAsync(instance);
                return instance;
            }

            task.Status = WorkflowTaskStatus.Approved;
            task.Comment = comment;
            await _tasks.UpdateAsync(task);
            await AddTimelineAsync(instance, "approved", task.StepId, actorUserId, comment);

            if (await QuorumMetAsync(instance, step))
            {
                await CancelPendingSiblingsAsync(instance, task.StepId, null);
                await AdvanceAsync(instance, versionFlow, step.NextStepId, actorUserId);
            }
            await _instances.UpdateAsync(instance);
            return instance;
        }

        /// <inheritdoc />
        public async Task<WorkflowSimulationResult> SimulateAsync(WorkflowVersion version, JsonElement formValues, long requesterUserId)
        {
            var flow = Deserialize<WorkflowFlowModel>(version.FlowJson);
            var result = new WorkflowSimulationResult();
            var current = flow.StartStepId;
            var guard = 0;
            while (!string.IsNullOrWhiteSpace(current) && guard++ < 64)
            {
                var step = flow.Steps.FirstOrDefault(s => s.Id == current)
                    ?? throw new UserFriendlyException($"Step '{current}' does not exist in version {version.Version}.");
                result.Path.Add(current);
                if (step.Type == "end")
                {
                    break;
                }
                if (step.Type == "approval")
                {
                    result.Assignees[current] = await ResolveAssigneesAsync(step.Assignee, requesterUserId, version.TenantId);
                }
                current = step.Type == "condition"
                    ? (EvaluateCondition(step.Condition, formValues) ? step.NextStepId : step.RejectStepId)
                    : step.NextStepId;
            }
            return result;
        }

        /// <inheritdoc />
        public async Task CancelAsync(WorkflowInstance instance, long actorUserId)
        {
            if (instance.Status != WorkflowInstanceStatus.InProgress && instance.Status != WorkflowInstanceStatus.SentBack)
            {
                throw new UserFriendlyException("Only a running request can be cancelled.");
            }
            instance.Status = WorkflowInstanceStatus.Cancelled;
            instance.CompletedTime = Clock.Now;
            await CancelPendingSiblingsAsync(instance, instance.CurrentStepId, null);
            await AddTimelineAsync(instance, "cancelled", instance.CurrentStepId, actorUserId, null);
            await _instances.UpdateAsync(instance);
        }

        /// <summary>Resumes a sent-back instance with updated values (BR-003).</summary>
        public async Task ResubmitAsync(WorkflowInstance instance, JsonElement formValues, long actorUserId)
        {
            if (instance.Status != WorkflowInstanceStatus.SentBack)
            {
                throw new UserFriendlyException("Only a sent-back request can be resubmitted.");
            }
            instance.FormValuesJson = formValues.GetRawText();
            instance.Status = WorkflowInstanceStatus.InProgress;
            await AddTimelineAsync(instance, "submitted", instance.CurrentStepId, actorUserId, null);

            var flow = await LoadFlowAsync(instance);
            await AdvanceAsync(instance, flow, instance.CurrentStepId, actorUserId);
            await _instances.UpdateAsync(instance);
        }

        private async Task<WorkflowFlowModel> LoadFlowAsync(WorkflowInstance instance)
        {
            var version = await _versions.FirstOrDefaultAsync(v =>
                v.DefinitionId == instance.DefinitionId && v.Version == instance.Version);
            if (version == null)
            {
                throw new UserFriendlyException($"Workflow version {instance.Version} not found for this instance.");
            }
            return Deserialize<WorkflowFlowModel>(version.FlowJson);
        }

        private async Task AdvanceAsync(WorkflowInstance instance, WorkflowFlowModel flow, string stepId, long actorUserId)
        {
            var guard = 0;
            var current = stepId;
            while (guard++ < 64)
            {
                var step = flow.Steps.FirstOrDefault(s => s.Id == current)
                    ?? throw new UserFriendlyException($"Step '{current}' does not exist.");
                instance.CurrentStepId = current;

                switch (step.Type)
                {
                    case "end":
                        instance.Status = WorkflowInstanceStatus.Completed;
                        instance.CompletedTime = Clock.Now;
                        await AddTimelineAsync(instance, "completed", current, actorUserId, null);
                        return;

                    case "condition":
                        var values = JsonDocument.Parse(instance.FormValuesJson ?? "{}").RootElement;
                        current = EvaluateCondition(step.Condition, values) ? step.NextStepId : step.RejectStepId;
                        await AddTimelineAsync(instance, "condition", step.Id, actorUserId, current);
                        continue;

                    case "notification":
                        await AddTimelineAsync(instance, "notified", step.Id, actorUserId, null);
                        current = step.NextStepId;
                        continue;

                    case "approval":
                        var assignees = await ResolveAssigneesAsync(step.Assignee, instance.RequesterUserId, instance.TenantId);
                        if (assignees.Count == 0)
                        {
                            throw new UserFriendlyException($"Step '{step.Id}': no assignees resolved.");
                        }
                        foreach (var userId in assignees)
                        {
                            await _tasks.InsertAsync(new WorkflowTask
                            {
                                Id = Guid.NewGuid(),
                                InstanceId = instance.Id,
                                StepId = step.Id,
                                AssigneeUserId = userId,
                                Status = WorkflowTaskStatus.Pending,
                                TenantId = instance.TenantId,
                                DueTime = step.DueTimeHours.HasValue
                                    ? Clock.Now.AddHours(step.DueTimeHours.Value)
                                    : null
                            });
                        }
                        await AddTimelineAsync(instance, "activated", step.Id, actorUserId,
                            JsonSerializer.Serialize(new { assignees }));
                        return;

                    default:
                        throw new UserFriendlyException($"Step '{step.Id}': unknown type '{step.Type}'.");
                }
            }
            throw new UserFriendlyException("Workflow exceeded the step limit — possible loop.");
        }

        private async Task<bool> QuorumMetAsync(WorkflowInstance instance, WorkflowStepModel step)
        {
            var approved = (await _tasks.GetAllListAsync(t =>
                t.InstanceId == instance.Id && t.StepId == step.Id && t.Status == WorkflowTaskStatus.Approved)).Count;

            var rule = step.Assignee?.Quorum ?? "all";
            switch (rule)
            {
                case "any":
                    return approved >= 1;
                case "quorum":
                    return approved >= Math.Max(1, step.Assignee.QuorumCount);
                default:
                    var total = (await _tasks.GetAllListAsync(t =>
                        t.InstanceId == instance.Id && t.StepId == step.Id)).Count;
                    return approved >= total;
            }
        }

        private async Task CancelPendingSiblingsAsync(WorkflowInstance instance, string stepId, Guid? exceptTaskId)
        {
            var pending = await _tasks.GetAllListAsync(t =>
                t.InstanceId == instance.Id && t.StepId == stepId && t.Status == WorkflowTaskStatus.Pending);
            foreach (var sibling in pending.Where(t => t.Id != exceptTaskId))
            {
                sibling.Status = WorkflowTaskStatus.Cancelled;
                await _tasks.UpdateAsync(sibling);
            }
        }

        /// <summary>Resolves an assignee rule to user ids (deterministic order for BR-004).</summary>
        public async Task<List<long>> ResolveAssigneesAsync(WorkflowAssigneeModel assignee, long requesterUserId, int? tenantId)
        {
            var result = new SortedSet<long>();
            if (assignee == null)
            {
                return result.ToList();
            }
            switch (assignee.Kind)
            {
                case "users":
                    foreach (var id in assignee.Ids ?? new List<string>())
                    {
                        if (long.TryParse(id, out var uid))
                        {
                            result.Add(uid);
                        }
                    }
                    break;
                case "role":
                    foreach (var role in assignee.Ids ?? new List<string>())
                    {
                        foreach (var uid in await _userDirectory.GetUsersInRoleAsync(role, tenantId))
                        {
                            result.Add(uid);
                        }
                    }
                    break;
                case "ou":
                    foreach (var ou in assignee.Ids ?? new List<string>())
                    {
                        foreach (var uid in await _userDirectory.GetUsersInOrganizationUnitAsync(ou, tenantId))
                        {
                            result.Add(uid);
                        }
                    }
                    break;
                case "manager":
                    var manager = await _userDirectory.GetManagerAsync(requesterUserId, tenantId);
                    if (manager.HasValue)
                    {
                        result.Add(manager.Value);
                    }
                    break;
            }
            return result.ToList();
        }

        /// <summary>Evaluates "field op value" against the submitted form values.</summary>
        public static bool EvaluateCondition(string expression, JsonElement values)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return false;
            }
            foreach (var op in new[] { " contains ", "!=", ">=", "<=", "==", ">", "<" })
            {
                var idx = expression.IndexOf(op, StringComparison.Ordinal);
                if (idx < 0)
                {
                    continue;
                }
                var field = expression[..idx].Trim();
                var expected = expression[(idx + op.Length)..].Trim().Trim('"', '\'');
                var actual = values.ValueKind == JsonValueKind.Object && values.TryGetProperty(field, out var v)
                    ? (v.ValueKind == JsonValueKind.String ? v.GetString() : v.GetRawText())
                    : null;

                return op.Trim() switch
                {
                    "==" => string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase),
                    "!=" => !string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase),
                    "contains" => actual != null && actual.Contains(expected, StringComparison.OrdinalIgnoreCase),
                    ">" or "<" or ">=" or "<=" => CompareNumbers(actual, expected, op.Trim()),
                    _ => false
                };
            }
            return false;
        }

        private static bool CompareNumbers(string actual, string expected, string op)
        {
            if (!decimal.TryParse(actual, out var a) || !decimal.TryParse(expected, out var b))
            {
                return false;
            }
            return op switch
            {
                ">" => a > b,
                "<" => a < b,
                ">=" => a >= b,
                "<=" => a <= b,
                _ => false
            };
        }

        private async Task AddTimelineAsync(WorkflowInstance instance, string eventType, string stepId, long? actorUserId, string dataJson)
        {
            await _timeline.InsertAsync(new WorkflowTimelineEntry
            {
                InstanceId = instance.Id,
                EventType = eventType,
                StepId = stepId,
                ActorUserId = actorUserId,
                DataJson = dataJson,
                TenantId = instance.TenantId
            });
        }

        private static T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
