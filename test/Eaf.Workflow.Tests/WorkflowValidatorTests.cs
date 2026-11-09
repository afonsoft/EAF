using Abp.UI;
using Eaf.Workflow.Domain;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Workflow.Tests
{
    /// <summary>
    /// Testes do validador de publicação de workflows.
    /// </summary>
    public class WorkflowValidatorTests : EafWorkflowTestBase
    {
        private readonly WorkflowValidator _validator;

        public WorkflowValidatorTests()
        {
            _validator = Resolve<WorkflowValidator>();
        }

        private static WorkflowFlowModel ValidFlow => new()
        {
            StartStepId = "s1",
            Steps = new List<WorkflowStepModel>
            {
                new() { Id = "s1", Type = "approval", NextStepId = "end", Assignee = new WorkflowAssigneeModel { Kind = "users", Ids = new List<string> { "1" } } },
                new() { Id = "end", Type = "end" }
            }
        };

        [Fact]
        public void Dado_FluxoValido_Quando_Validar_Entao_Passa()
        {
            Should.NotThrow(() => _validator.ValidatePublish(new WorkflowFormModel(), ValidFlow));
        }

        [Fact]
        public void Dado_SemEtapaInicial_Quando_Validar_Entao_FalhaNomeando()
        {
            var flow = ValidFlow;
            flow.StartStepId = "missing";
            var ex = Should.Throw<UserFriendlyException>(() => _validator.ValidatePublish(null, flow));
            ex.Message.ShouldContain("missing");
        }

        [Fact]
        public void Dado_StepSemAssignee_Quando_Validar_Entao_FalhaNomeandoStep()
        {
            var flow = ValidFlow;
            flow.Steps[0].Assignee = null;
            var ex = Should.Throw<UserFriendlyException>(() => _validator.ValidatePublish(null, flow));
            ex.Message.ShouldContain("s1");
        }

        [Fact]
        public void Dado_StepOrfao_Quando_Validar_Entao_FalhaNomeandoStep()
        {
            var flow = ValidFlow;
            flow.Steps.Add(new WorkflowStepModel { Id = "ghost", Type = "end" });
            var ex = Should.Throw<UserFriendlyException>(() => _validator.ValidatePublish(null, flow));
            ex.Message.ShouldContain("ghost");
        }

        [Fact]
        public void Dado_IdDuplicado_Quando_Validar_Entao_Falha()
        {
            var flow = ValidFlow;
            flow.Steps[1].Id = "s1";
            flow.Steps[0].NextStepId = "s1"; // loop to itself also fine — duplicate triggers first
            var ex = Should.Throw<UserFriendlyException>(() => _validator.ValidatePublish(null, flow));
            ex.Message.ShouldContain("Duplicate step id");
        }

        [Fact]
        public void Dado_SemEnd_Quando_Validar_Entao_Falha()
        {
            var flow = ValidFlow;
            flow.Steps.RemoveAt(1);
            var ex = Should.Throw<UserFriendlyException>(() => _validator.ValidatePublish(null, flow));
            ex.Message.ShouldContain("no 'end' step");
        }

        [Fact]
        public void Dado_CondicaoSemRamo_Quando_Validar_Entao_Falha()
        {
            var flow = ValidFlow;
            flow.Steps[0].Type = "condition";
            flow.Steps[0].Condition = "a == b";
            flow.Steps[0].RejectStepId = null;
            var ex = Should.Throw<UserFriendlyException>(() => _validator.ValidatePublish(null, flow));
            ex.Message.ShouldContain("rejectStepId");
        }
    }
}
