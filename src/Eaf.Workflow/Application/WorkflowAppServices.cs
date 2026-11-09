using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Eaf.Workflow.Application.Dto;
using Eaf.Workflow.Authorization;
using Eaf.Workflow.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eaf.Workflow.Application
{
    /// <summary>
    /// Admin CRUD + publish for workflow definitions.
    /// </summary>
    [AbpAuthorize(EafWorkflowPermissions.Pages_Administration_Workflows)]
    public class WorkflowDefinitionAppService : ApplicationService
    {
        private readonly IRepository<WorkflowDefinition, Guid> _definitions;
        private readonly IRepository<WorkflowVersion, Guid> _versions;
        private readonly WorkflowValidator _validator;

        /// <summary>WorkflowDefinitionAppService.</summary>
        public WorkflowDefinitionAppService(
            IRepository<WorkflowDefinition, Guid> definitions,
            IRepository<WorkflowVersion, Guid> versions,
            WorkflowValidator validator)
        {
            _definitions = definitions;
            _versions = versions;
            _validator = validator;
        }

        /// <summary>Lista definições do tenant.</summary>
        public async Task<ListResultDto<WorkflowDefinitionDto>> GetAllAsync()
        {
            var defs = await _definitions.GetAllListAsync();
            return new ListResultDto<WorkflowDefinitionDto>(
                defs.OrderBy(d => d.Name).Select(Map).ToList());
        }

        /// <summary>Obtém definição para edição.</summary>
        public async Task<WorkflowDefinitionDto> GetAsync(Guid id)
        {
            return Map(await _definitions.GetAsync(id));
        }

        /// <summary>Cria definição em rascunho.</summary>
        [AbpAuthorize(EafWorkflowPermissions.Pages_Administration_Workflows_Create)]
        public async Task<WorkflowDefinitionDto> CreateAsync(CreateOrUpdateWorkflowDefinitionInput input)
        {
            var def = new WorkflowDefinition
            {
                Id = Guid.NewGuid(),
                Name = input.Name,
                Description = input.Description,
                Category = input.Category,
                FormJson = input.FormJson ?? "{}",
                FlowJson = input.FlowJson ?? "{}",
                Status = WorkflowStatus.Draft,
                TenantId = AbpSession.TenantId
            };
            await _definitions.InsertAsync(def);
            return Map(def);
        }

        /// <summary>Edita definição — apenas rascunhos (BR-001).</summary>
        [AbpAuthorize(EafWorkflowPermissions.Pages_Administration_Workflows_Edit)]
        public async Task<WorkflowDefinitionDto> UpdateAsync(CreateOrUpdateWorkflowDefinitionInput input)
        {
            var def = await _definitions.GetAsync(input.Id);
            if (def.Status == WorkflowStatus.Published)
            {
                throw new UserFriendlyException("Published definitions are immutable — archive it or create a new draft.");
            }
            def.Name = input.Name;
            def.Description = input.Description;
            def.Category = input.Category;
            def.FormJson = input.FormJson ?? def.FormJson;
            def.FlowJson = input.FlowJson ?? def.FlowJson;
            await _definitions.UpdateAsync(def);
            return Map(def);
        }

        /// <summary>Publica: valida, bumpa versão, snapshot imutável.</summary>
        [AbpAuthorize(EafWorkflowPermissions.Pages_Administration_Workflows_Publish)]
        public async Task<WorkflowDefinitionDto> PublishAsync(Guid id)
        {
            var def = await _definitions.GetAsync(id);
            var form = Deserialize<WorkflowFormModel>(def.FormJson);
            var flow = Deserialize<WorkflowFlowModel>(def.FlowJson);
            _validator.ValidatePublish(form, flow);

            def.Version += 1;
            def.Status = WorkflowStatus.Published;
            await _definitions.UpdateAsync(def);

            await _versions.InsertAsync(new WorkflowVersion
            {
                Id = Guid.NewGuid(),
                DefinitionId = def.Id,
                Version = def.Version,
                FormJson = def.FormJson,
                FlowJson = def.FlowJson,
                TenantId = def.TenantId
            });
            return Map(def);
        }

        /// <summary>Arquiva definição (instâncias continuam).</summary>
        [AbpAuthorize(EafWorkflowPermissions.Pages_Administration_Workflows_Edit)]
        public async Task ArchiveAsync(Guid id)
        {
            var def = await _definitions.GetAsync(id);
            def.Status = WorkflowStatus.Archived;
            await _definitions.UpdateAsync(def);
        }

        /// <summary>Exclui definição.</summary>
        [AbpAuthorize(EafWorkflowPermissions.Pages_Administration_Workflows_Delete)]
        public async Task DeleteAsync(Guid id)
        {
            await _definitions.DeleteAsync(id);
        }

        private static WorkflowDefinitionDto Map(WorkflowDefinition d) => new()
        {
            Id = d.Id,
            Name = d.Name,
            Description = d.Description,
            Category = d.Category,
            Status = d.Status.ToString(),
            Version = d.Version,
            FormJson = d.FormJson,
            FlowJson = d.FlowJson
        };

        internal static T Deserialize<T>(string json) =>
            string.IsNullOrWhiteSpace(json)
                ? default
                : JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    /// <summary>
    /// Requester-facing catalog + start + my requests + instance detail.
    /// </summary>
    [AbpAuthorize(EafWorkflowPermissions.Pages_Workflows)]
    public class WorkflowRequestAppService : ApplicationService
    {
        private readonly IRepository<WorkflowDefinition, Guid> _definitions;
        private readonly IRepository<WorkflowVersion, Guid> _versions;
        private readonly IRepository<WorkflowInstance, Guid> _instances;
        private readonly IRepository<WorkflowTask, Guid> _tasks;
        private readonly IRepository<WorkflowTimelineEntry, long> _timeline;
        private readonly IWorkflowEngine _engine;

        /// <summary>WorkflowRequestAppService.</summary>
        public WorkflowRequestAppService(
            IRepository<WorkflowDefinition, Guid> definitions,
            IRepository<WorkflowVersion, Guid> versions,
            IRepository<WorkflowInstance, Guid> instances,
            IRepository<WorkflowTask, Guid> tasks,
            IRepository<WorkflowTimelineEntry, long> timeline,
            IWorkflowEngine engine)
        {
            _definitions = definitions;
            _versions = versions;
            _instances = instances;
            _tasks = tasks;
            _timeline = timeline;
            _engine = engine;
        }

        /// <summary>Catálogo: definições publicadas.</summary>
        public async Task<ListResultDto<WorkflowDefinitionDto>> GetCatalogAsync()
        {
            await EnsureWorkflowsEnabledAsync();
            var defs = await _definitions.GetAllListAsync(d => d.Status == WorkflowStatus.Published);
            return new ListResultDto<WorkflowDefinitionDto>(
                defs.OrderBy(d => d.Category).ThenBy(d => d.Name).Select(d => new WorkflowDefinitionDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    Category = d.Category,
                    Status = d.Status.ToString(),
                    Version = d.Version
                }).ToList());
        }

        /// <summary>Submete uma solicitação a partir do catálogo.</summary>
        public async Task<WorkflowInstanceDto> StartAsync(StartWorkflowInput input)
        {
            await EnsureWorkflowsEnabledAsync();
            var def = await _definitions.GetAsync(input.DefinitionId);
            if (def.Status != WorkflowStatus.Published)
            {
                throw new UserFriendlyException("This workflow is not published.");
            }
            var version = await _versions.FirstOrDefaultAsync(v =>
                v.DefinitionId == def.Id && v.Version == def.Version)
                ?? throw new UserFriendlyException("Published version not found.");

            using var doc = JsonDocument.Parse(input.FormValuesJson ?? "{}");
            var instance = await _engine.StartAsync(version, AbpSession.UserId.GetValueOrDefault(), doc.RootElement.Clone(), AbpSession.TenantId);
            return MapInstance(instance);
        }

        /// <summary>Dry-run: retorna o caminho + aprovadores sem persistir.</summary>
        public async Task<WorkflowSimulationDto> SimulateAsync(StartWorkflowInput input)
        {
            var def = await _definitions.GetAsync(input.DefinitionId);
            var version = await _versions.FirstOrDefaultAsync(v =>
                v.DefinitionId == def.Id && v.Version == def.Version)
                ?? throw new UserFriendlyException("Published version not found.");

            using var doc = JsonDocument.Parse(input.FormValuesJson ?? "{}");
            var result = await _engine.SimulateAsync(version, doc.RootElement.Clone(), AbpSession.UserId.GetValueOrDefault());
            return new WorkflowSimulationDto
            {
                Path = result.Path,
                Assignees = result.Assignees.ToDictionary(kv => kv.Key, kv => kv.Value)
            };
        }

        /// <summary>Minhas solicitações.</summary>
        public async Task<ListResultDto<WorkflowInstanceDto>> GetMyRequestsAsync()
        {
            var list = await _instances.GetAllListAsync(i => i.RequesterUserId == AbpSession.UserId.GetValueOrDefault());
            return new ListResultDto<WorkflowInstanceDto>(
                list.OrderByDescending(i => i.CreationTime).Select(MapInstance).ToList());
        }

        /// <summary>Detalhe com timeline e tarefas.</summary>
        public async Task<WorkflowInstanceDetailDto> GetDetailAsync(Guid id)
        {
            var instance = await _instances.GetAsync(id);
            var tasks = await _tasks.GetAllListAsync(t => t.InstanceId == id);
            var entries = await _timeline.GetAllListAsync(t => t.InstanceId == id);
            return new WorkflowInstanceDetailDto
            {
                Instance = MapInstance(instance),
                Tasks = tasks.OrderBy(t => t.CreationTime).Select(MapTask).ToList(),
                Timeline = entries.OrderBy(e => e.CreationTime).Select(e => new WorkflowTimelineEntryDto
                {
                    EventType = e.EventType,
                    StepId = e.StepId,
                    ActorUserId = e.ActorUserId,
                    DataJson = e.DataJson,
                    CreationTime = e.CreationTime
                }).ToList()
            };
        }

        /// <summary>Cancela solicitação em andamento.</summary>
        public async Task CancelAsync(Guid id)
        {
            var instance = await _instances.GetAsync(id);
            if (instance.RequesterUserId != AbpSession.UserId.GetValueOrDefault())
            {
                throw new UserFriendlyException("Only the requester can cancel this request.");
            }
            await _engine.CancelAsync(instance, AbpSession.UserId.GetValueOrDefault());
        }

        /// <summary>Reenvia solicitação devolvida (BR-003).</summary>
        public async Task<WorkflowInstanceDto> ResubmitAsync(ResubmitWorkflowInput input)
        {
            var instance = await _instances.GetAsync(input.InstanceId);
            if (instance.RequesterUserId != AbpSession.UserId.GetValueOrDefault())
            {
                throw new UserFriendlyException("Only the requester can resubmit this request.");
            }
            using var doc = JsonDocument.Parse(input.FormValuesJson ?? "{}");
            if (_engine is WorkflowEngine engine)
            {
                await engine.ResubmitAsync(instance, doc.RootElement.Clone(), AbpSession.UserId.GetValueOrDefault());
            }
            return MapInstance(instance);
        }

        private async Task EnsureWorkflowsEnabledAsync()
        {
            var enabled = await SettingManager.GetSettingValueAsync(WorkflowConsts.IsEnabledSettingName);
            if (!string.Equals(enabled, "true", System.StringComparison.OrdinalIgnoreCase))
            {
                throw new UserFriendlyException("Workflows are disabled for this tenant.");
            }
        }

        private static WorkflowInstanceDto MapInstance(WorkflowInstance i) => new()
        {
            Id = i.Id,
            DefinitionId = i.DefinitionId,
            Version = i.Version,
            RequesterUserId = i.RequesterUserId,
            FormValuesJson = i.FormValuesJson,
            CurrentStepId = i.CurrentStepId,
            Status = i.Status.ToString(),
            CreationTime = i.CreationTime,
            CompletedTime = i.CompletedTime
        };

        internal static WorkflowTaskDto MapTask(WorkflowTask t) => new()
        {
            Id = t.Id,
            InstanceId = t.InstanceId,
            StepId = t.StepId,
            AssigneeUserId = t.AssigneeUserId,
            Status = t.Status.ToString(),
            Comment = t.Comment,
            DueTime = t.DueTime,
            CreationTime = t.CreationTime
        };
    }

    /// <summary>
    /// Approval inbox — pending tasks + approve/reject/send-back.
    /// </summary>
    [AbpAuthorize(EafWorkflowPermissions.Pages_Workflows_Approvals)]
    public class WorkflowApprovalAppService : ApplicationService
    {
        private readonly IRepository<WorkflowTask, Guid> _tasks;
        private readonly IRepository<WorkflowInstance, Guid> _instances;
        private readonly IWorkflowEngine _engine;
        private readonly IWorkflowUserDirectory _directory;

        /// <summary>WorkflowApprovalAppService.</summary>
        public WorkflowApprovalAppService(
            IRepository<WorkflowTask, Guid> tasks,
            IRepository<WorkflowInstance, Guid> instances,
            IWorkflowEngine engine,
            IWorkflowUserDirectory directory)
        {
            _tasks = tasks;
            _instances = instances;
            _engine = engine;
            _directory = directory;
        }

        /// <summary>Tarefas pendentes atribuídas a mim (incluindo delegações ativas a meu favor).</summary>
        public async Task<ListResultDto<WorkflowTaskDto>> GetMyPendingAsync()
        {
            var allowed = await AllowedAssigneeIdsAsync();
            var list = await _tasks.GetAllListAsync(t => allowed.Contains(t.AssigneeUserId) && t.Status == WorkflowTaskStatus.Pending);
            return new ListResultDto<WorkflowTaskDto>(
                list.OrderBy(t => t.CreationTime).Select(WorkflowRequestAppService.MapTask).ToList());
        }

        /// <summary>Aprova tarefa.</summary>
        public async Task DecideAsync(DecideWorkflowInput input)
        {
            var task = await _tasks.GetAsync(input.TaskId);
            var allowed = await AllowedAssigneeIdsAsync();
            if (!allowed.Contains(task.AssigneeUserId))
            {
                throw new UserFriendlyException("This task is not assigned to you.");
            }
            var decision = input.Decision switch
            {
                "approve" => WorkflowTaskStatus.Approved,
                "reject" => WorkflowTaskStatus.Rejected,
                "send-back" => WorkflowTaskStatus.SentBack,
                _ => throw new UserFriendlyException("Decision must be approve, reject or send-back.")
            };
            await _engine.DecideAsync(task, AbpSession.UserId.GetValueOrDefault(), decision, input.Comment);
        }

        private async Task<List<long>> AllowedAssigneeIdsAsync()
        {
            var me = AbpSession.UserId.GetValueOrDefault();
            var delegators = await _directory.GetActiveDelegatorIdsAsync(me, AbpSession.TenantId);
            var allowed = new List<long> { me };
            allowed.AddRange(delegators);
            return allowed;
        }
    }
}
