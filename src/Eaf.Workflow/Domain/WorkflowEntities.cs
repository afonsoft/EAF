using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eaf.Workflow.Domain
{
    /// <summary>Lifecycle of a workflow definition.</summary>
    public enum WorkflowStatus
    {
        /// <summary>Editable, not startable.</summary>
        Draft = 0,
        /// <summary>Startable from the catalog; immutable.</summary>
        Published = 1,
        /// <summary>Retired; existing instances keep running.</summary>
        Archived = 2
    }

    /// <summary>Lifecycle of a running instance.</summary>
    public enum WorkflowInstanceStatus
    {
        /// <summary>Waiting on a step.</summary>
        InProgress = 0,
        /// <summary>Reached the end step.</summary>
        Completed = 1,
        /// <summary>Rejected by an approver.</summary>
        Rejected = 2,
        /// <summary>Returned to the requester for changes.</summary>
        SentBack = 3,
        /// <summary>Cancelled by the requester.</summary>
        Cancelled = 4
    }

    /// <summary>Lifecycle of one approval task.</summary>
    public enum WorkflowTaskStatus
    {
        /// <summary>Awaiting decision.</summary>
        Pending = 0,
        /// <summary>Approved.</summary>
        Approved = 1,
        /// <summary>Rejected.</summary>
        Rejected = 2,
        /// <summary>Sent back for changes.</summary>
        SentBack = 3,
        /// <summary>Cancelled (step moved on or instance closed).</summary>
        Cancelled = 4
    }

    /// <summary>
    /// Versioned workflow definition: name + form schema + flow graph.
    /// Published definitions are immutable — a publish snapshots <see cref="WorkflowVersion"/>.
    /// </summary>
    [Table("EafWorkflowDefinitions")]
    public class WorkflowDefinition : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>Tamanho máximo do nome.</summary>
        public const int MaxNameLength = 128;

        /// <summary>Nome exibido no catálogo.</summary>
        [Required]
        [StringLength(MaxNameLength)]
        public string Name { get; set; }

        /// <summary>Descrição do propósito.</summary>
        [StringLength(1024)]
        public string Description { get; set; }

        /// <summary>Categoria de agrupamento no catálogo.</summary>
        [StringLength(64)]
        public string Category { get; set; }

        /// <summary>Status de ciclo de vida.</summary>
        public WorkflowStatus Status { get; set; } = WorkflowStatus.Draft;

        /// <summary>Versão publicada corrente (0 = nunca publicada).</summary>
        public int Version { get; set; }

        /// <summary>Schema do formulário em JSON (rascunho editável).</summary>
        public string FormJson { get; set; }

        /// <summary>Grafo de etapas em JSON (rascunho editável).</summary>
        public string FlowJson { get; set; }

        /// <summary>Tenant da definição.</summary>
        public int? TenantId { get; set; }
    }

    /// <summary>
    /// Immutable snapshot of a published definition. Instances pin this version.
    /// </summary>
    [Table("EafWorkflowVersions")]
    public class WorkflowVersion : CreationAuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>Definição dona.</summary>
        public Guid DefinitionId { get; set; }

        /// <summary>Número da versão.</summary>
        public int Version { get; set; }

        /// <summary>Snapshot do formulário.</summary>
        public string FormJson { get; set; }

        /// <summary>Snapshot do grafo.</summary>
        public string FlowJson { get; set; }

        /// <summary>Tenant.</summary>
        public int? TenantId { get; set; }
    }

    /// <summary>
    /// One running request.
    /// </summary>
    [Table("EafWorkflowInstances")]
    public class WorkflowInstance : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>Definição de origem.</summary>
        public Guid DefinitionId { get; set; }

        /// <summary>Versão pinada no início (BR-002).</summary>
        public int Version { get; set; }

        /// <summary>Usuário que submeteu.</summary>
        public long RequesterUserId { get; set; }

        /// <summary>Valores do formulário em JSON.</summary>
        public string FormValuesJson { get; set; }

        /// <summary>Etapa corrente (id dentro do grafo da versão).</summary>
        [StringLength(64)]
        public string CurrentStepId { get; set; }

        /// <summary>Status da instância.</summary>
        public WorkflowInstanceStatus Status { get; set; } = WorkflowInstanceStatus.InProgress;

        /// <summary>Término (complete/reject/cancel).</summary>
        public DateTime? CompletedTime { get; set; }

        /// <summary>Tenant.</summary>
        public int? TenantId { get; set; }
    }

    /// <summary>
    /// One approval unit — resolved assignee at activation time.
    /// </summary>
    [Table("EafWorkflowTasks")]
    public class WorkflowTask : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        /// <summary>Instância dona.</summary>
        public Guid InstanceId { get; set; }

        /// <summary>Etapa que gerou a tarefa.</summary>
        [Required]
        [StringLength(64)]
        public string StepId { get; set; }

        /// <summary>Aprovador resolvido.</summary>
        public long AssigneeUserId { get; set; }

        /// <summary>Status da tarefa.</summary>
        public WorkflowTaskStatus Status { get; set; } = WorkflowTaskStatus.Pending;

        /// <summary>Comentário da decisão.</summary>
        [StringLength(2000)]
        public string Comment { get; set; }

        /// <summary>Prazo (SLA — Phase B).</summary>
        public DateTime? DueTime { get; set; }

        /// <summary>Tenant.</summary>
        public int? TenantId { get; set; }
    }

    /// <summary>
    /// Append-only audit entry — every submission, decision, transition.
    /// </summary>
    [Table("EafWorkflowTimeline")]
    public class WorkflowTimelineEntry : CreationAuditedEntity<long>, IMayHaveTenant
    {
        /// <summary>Instância.</summary>
        public Guid InstanceId { get; set; }

        /// <summary>Tipo do evento (submitted, activated, approved, rejected, sent-back, cancelled, completed).</summary>
        [Required]
        [StringLength(32)]
        public string EventType { get; set; }

        /// <summary>Etapa relacionada.</summary>
        [StringLength(64)]
        public string StepId { get; set; }

        /// <summary>Ator.</summary>
        public long? ActorUserId { get; set; }

        /// <summary>Dados extras em JSON.</summary>
        public string DataJson { get; set; }

        /// <summary>Tenant.</summary>
        public int? TenantId { get; set; }
    }
}
