using Abp.Application.Services.Dto;
using Eaf.Workflow.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Workflow.Application.Dto
{
    /// <summary>DTO de definição de workflow.</summary>
    public class WorkflowDefinitionDto : EntityDto<Guid>
    {
        /// <summary>Nome.</summary>
        public string Name { get; set; }
        /// <summary>Descrição.</summary>
        public string Description { get; set; }
        /// <summary>Categoria.</summary>
        public string Category { get; set; }
        /// <summary>Status (Draft/Published/Archived).</summary>
        public string Status { get; set; }
        /// <summary>Versão.</summary>
        public int Version { get; set; }
        /// <summary>Schema do formulário (JSON).</summary>
        public string FormJson { get; set; }
        /// <summary>Grafo de etapas (JSON).</summary>
        public string FlowJson { get; set; }
    }

    /// <summary>Input de criação/edição de definição.</summary>
    public class CreateOrUpdateWorkflowDefinitionInput : EntityDto<Guid>
    {
        /// <summary>Nome.</summary>
        [Required]
        [StringLength(WorkflowDefinition.MaxNameLength)]
        public string Name { get; set; }
        /// <summary>Descrição.</summary>
        public string Description { get; set; }
        /// <summary>Categoria.</summary>
        public string Category { get; set; }
        /// <summary>Form JSON.</summary>
        public string FormJson { get; set; }
        /// <summary>Flow JSON.</summary>
        public string FlowJson { get; set; }
    }

    /// <summary>Input para iniciar solicitação.</summary>
    public class StartWorkflowInput
    {
        /// <summary>Definição publicada.</summary>
        [Required]
        public Guid DefinitionId { get; set; }
        /// <summary>Valores do formulário (JSON).</summary>
        public string FormValuesJson { get; set; }
    }

    /// <summary>Resultado da simulação.</summary>
    public class WorkflowSimulationDto
    {
        /// <summary>Caminho de etapas.</summary>
        public List<string> Path { get; set; } = new();
        /// <summary>Aprovadores resolvidos por etapa.</summary>
        public Dictionary<string, List<long>> Assignees { get; set; } = new();
    }

    /// <summary>DTO de instância.</summary>
    public class WorkflowInstanceDto : EntityDto<Guid>
    {
        /// <summary>Definição.</summary>
        public Guid DefinitionId { get; set; }
        /// <summary>Versão pinada.</summary>
        public int Version { get; set; }
        /// <summary>Requester.</summary>
        public long RequesterUserId { get; set; }
        /// <summary>Valores (JSON).</summary>
        public string FormValuesJson { get; set; }
        /// <summary>Etapa corrente.</summary>
        public string CurrentStepId { get; set; }
        /// <summary>Status.</summary>
        public string Status { get; set; }
        /// <summary>Criação.</summary>
        public DateTime CreationTime { get; set; }
        /// <summary>Término.</summary>
        public DateTime? CompletedTime { get; set; }
    }

    /// <summary>Detalhe da instância.</summary>
    public class WorkflowInstanceDetailDto
    {
        /// <summary>Instância.</summary>
        public WorkflowInstanceDto Instance { get; set; }
        /// <summary>Tarefas.</summary>
        public List<WorkflowTaskDto> Tasks { get; set; } = new();
        /// <summary>Timeline.</summary>
        public List<WorkflowTimelineEntryDto> Timeline { get; set; } = new();
    }

    /// <summary>DTO de tarefa.</summary>
    public class WorkflowTaskDto : EntityDto<Guid>
    {
        /// <summary>Instância.</summary>
        public Guid InstanceId { get; set; }
        /// <summary>Etapa.</summary>
        public string StepId { get; set; }
        /// <summary>Aprovador.</summary>
        public long AssigneeUserId { get; set; }
        /// <summary>Status.</summary>
        public string Status { get; set; }
        /// <summary>Comentário.</summary>
        public string Comment { get; set; }
        /// <summary>SLA.</summary>
        public DateTime? DueTime { get; set; }
        /// <summary>Criação.</summary>
        public DateTime CreationTime { get; set; }
    }

    /// <summary>Entrada da timeline.</summary>
    public class WorkflowTimelineEntryDto
    {
        /// <summary>Tipo do evento.</summary>
        public string EventType { get; set; }
        /// <summary>Etapa.</summary>
        public string StepId { get; set; }
        /// <summary>Ator.</summary>
        public long? ActorUserId { get; set; }
        /// <summary>Dados (JSON).</summary>
        public string DataJson { get; set; }
        /// <summary>Quando.</summary>
        public DateTime CreationTime { get; set; }
    }

    /// <summary>Decisão sobre uma tarefa.</summary>
    public class DecideWorkflowInput
    {
        /// <summary>Tarefa.</summary>
        [Required]
        public Guid TaskId { get; set; }
        /// <summary>approve | reject | send-back.</summary>
        [Required]
        public string Decision { get; set; }
        /// <summary>Comentário.</summary>
        public string Comment { get; set; }
    }

    /// <summary>Reenvio de solicitação devolvida.</summary>
    public class ResubmitWorkflowInput
    {
        /// <summary>Instância.</summary>
        [Required]
        public Guid InstanceId { get; set; }
        /// <summary>Novos valores (JSON).</summary>
        public string FormValuesJson { get; set; }
    }
}
