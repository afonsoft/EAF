using System.Collections.Generic;

namespace Eaf.Workflow.Domain
{
    /// <summary>
    /// Serialized form schema — a list of typed fields the requester fills in.
    /// </summary>
    public class WorkflowFormModel
    {
        /// <summary>Campos do formulário.</summary>
        public List<WorkflowFormField> Fields { get; set; } = new();
    }

    /// <summary>One form field.</summary>
    public class WorkflowFormField
    {
        /// <summary>Identificador único no formulário.</summary>
        public string Id { get; set; }

        /// <summary>Rótulo exibido.</summary>
        public string Label { get; set; }

        /// <summary>Tipo: text | number | money | date | selection | user | attachment.</summary>
        public string Type { get; set; } = "text";

        /// <summary>Campo obrigatório.</summary>
        public bool Required { get; set; }

        /// <summary>Opções (quando type=selection).</summary>
        public List<string> Options { get; set; } = new();
    }

    /// <summary>
    /// Serialized step graph. The first step is <see cref="StartStepId"/>; a run ends on a step of type "end".
    /// </summary>
    public class WorkflowFlowModel
    {
        /// <summary>Id da etapa inicial.</summary>
        public string StartStepId { get; set; }

        /// <summary>Etapas do grafo.</summary>
        public List<WorkflowStepModel> Steps { get; set; } = new();
    }

    /// <summary>One graph node.</summary>
    public class WorkflowStepModel
    {
        /// <summary>Id único no grafo.</summary>
        public string Id { get; set; }

        /// <summary>Nome amigável.</summary>
        public string Name { get; set; }

        /// <summary>Tipo: approval | condition | notification | end.</summary>
        public string Type { get; set; }

        /// <summary>Próxima etapa (aprovado/transição linear).</summary>
        public string NextStepId { get; set; }

        /// <summary>Etapa quando rejeitado / condição falsa.</summary>
        public string RejectStepId { get; set; }

        /// <summary>Expressão da condição ("campo == valor", "&gt;", "contains"...). Avaliada sobre FormValues.</summary>
        public string Condition { get; set; }

        /// <summary>Regra de atribuição (approval).</summary>
        public WorkflowAssigneeModel Assignee { get; set; }

        /// <summary>SLA: horas até o vencimento da tarefa a partir da ativação (null = sem SLA).</summary>
        public double? DueTimeHours { get; set; }
    }

    /// <summary>Who approves a step.</summary>
    public class WorkflowAssigneeModel
    {
        /// <summary>Tipo de alvo: users | role | ou | manager.</summary>
        public string Kind { get; set; } = "users";

        /// <summary>Ids do alvo (user ids, role name, ou code).</summary>
        public List<string> Ids { get; set; } = new();

        /// <summary>Quórum: all | any | quorum.</summary>
        public string Quorum { get; set; } = "all";

        /// <summary>N necessário quando quorum=quorum.</summary>
        public int QuorumCount { get; set; } = 1;
    }
}
