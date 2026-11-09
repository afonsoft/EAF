using Abp.Dependency;
using Abp.Domain.Services;
using Abp.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Eaf.Workflow.Domain
{
    /// <summary>
    /// Publish-time validator — rejects a definition with exact-step diagnostics.
    /// </summary>
    public class WorkflowValidator : DomainService
    {
        /// <summary>
        /// Validates a flow model. Throws <see cref="UserFriendlyException"/> naming the broken step.
        /// </summary>
        public void ValidatePublish(WorkflowFormModel form, WorkflowFlowModel flow)
        {
            if (flow == null || flow.Steps == null || flow.Steps.Count == 0)
            {
                throw new UserFriendlyException("Workflow has no steps.");
            }
            var ids = new HashSet<string>();
            foreach (var step in flow.Steps)
            {
                if (string.IsNullOrWhiteSpace(step.Id))
                {
                    throw new UserFriendlyException("A workflow step has an empty id.");
                }
                if (!ids.Add(step.Id))
                {
                    throw new UserFriendlyException($"Duplicate step id '{step.Id}'.");
                }
            }
            if (string.IsNullOrWhiteSpace(flow.StartStepId) || !ids.Contains(flow.StartStepId))
            {
                throw new UserFriendlyException($"Start step '{flow.StartStepId}' does not exist.");
            }
            if (!flow.Steps.Any(s => s.Type == "end"))
            {
                throw new UserFriendlyException("Workflow has no 'end' step.");
            }

            foreach (var step in flow.Steps)
            {
                switch (step.Type)
                {
                    case "approval":
                        if (step.Assignee == null || (step.Assignee.Kind != "manager" && (step.Assignee.Ids == null || step.Assignee.Ids.Count == 0)))
                        {
                            throw new UserFriendlyException($"Step '{step.Id}': approval step requires an assignee.");
                        }
                        RequireNext(flow, ids, step);
                        if (!string.IsNullOrWhiteSpace(step.RejectStepId) && !ids.Contains(step.RejectStepId))
                        {
                            throw new UserFriendlyException($"Step '{step.Id}': reject target '{step.RejectStepId}' does not exist.");
                        }
                        break;
                    case "condition":
                        if (string.IsNullOrWhiteSpace(step.Condition))
                        {
                            throw new UserFriendlyException($"Step '{step.Id}': condition step requires an expression.");
                        }
                        RequireNext(flow, ids, step);
                        if (string.IsNullOrWhiteSpace(step.RejectStepId) || !ids.Contains(step.RejectStepId))
                        {
                            throw new UserFriendlyException($"Step '{step.Id}': condition step requires a valid 'rejectStepId'.");
                        }
                        break;
                    case "notification":
                        RequireNext(flow, ids, step);
                        break;
                    case "end":
                        break;
                    default:
                        throw new UserFriendlyException($"Step '{step.Id}': unknown type '{step.Type}'.");
                }
            }

            // Orphan detection: every step must be reachable from start.
            var reachable = new HashSet<string> { flow.StartStepId };
            var queue = new Queue<string>();
            queue.Enqueue(flow.StartStepId);
            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                var current = flow.Steps.First(s => s.Id == currentId);
                foreach (var next in new[] { current.NextStepId, current.RejectStepId })
                {
                    if (!string.IsNullOrWhiteSpace(next) && reachable.Add(next))
                    {
                        queue.Enqueue(next);
                    }
                }
            }
            var orphan = flow.Steps.FirstOrDefault(s => !reachable.Contains(s.Id));
            if (orphan != null)
            {
                throw new UserFriendlyException($"Step '{orphan.Id}' is unreachable from '{flow.StartStepId}'.");
            }
        }

        /// <summary>
        /// Validates submitted form values against the schema; returns missing required field labels.
        /// </summary>
        public List<string> ValidateFormValues(WorkflowFormModel form, JsonElement values)
        {
            var missing = new List<string>();
            if (form?.Fields == null)
            {
                return missing;
            }
            foreach (var field in form.Fields.Where(f => f.Required))
            {
                var present = values.ValueKind == JsonValueKind.Object
                    && values.TryGetProperty(field.Id, out var v)
                    && v.ValueKind != JsonValueKind.Null
                    && (v.ValueKind != JsonValueKind.String || !string.IsNullOrWhiteSpace(v.GetString()));
                if (!present)
                {
                    missing.Add(field.Label ?? field.Id);
                }
            }
            return missing;
        }

        private static void RequireNext(WorkflowFlowModel flow, HashSet<string> ids, WorkflowStepModel step)
        {
            if (string.IsNullOrWhiteSpace(step.NextStepId) || !ids.Contains(step.NextStepId))
            {
                throw new UserFriendlyException($"Step '{step.Id}': 'nextStepId' is missing or invalid.");
            }
        }
    }
}
