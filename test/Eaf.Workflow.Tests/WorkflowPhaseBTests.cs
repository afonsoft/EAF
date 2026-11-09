using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Eaf.Workflow.Application;
using Eaf.Workflow.Application.Dto;
using Eaf.Workflow.Domain;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Workflow.Tests
{
    /// <summary>
    /// Testes da Phase B: SLA nas tarefas e delegações na caixa de aprovação.
    /// </summary>
    public class WorkflowPhaseBTests : EafWorkflowTestBase
    {
        private static WorkflowApprovalAppService CreateApprovalService(
            List<WorkflowTask> tasks,
            long currentUserId,
            List<long> delegatorIds,
            out IWorkflowEngine engine)
        {
            var taskStore = tasks;
            var taskRepo = Substitute.For<IRepository<WorkflowTask, Guid>>();
            taskRepo.GetAllListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<WorkflowTask, bool>>>())
                .Returns(ci => Task.FromResult(
                    taskStore.AsQueryable().Where(ci.Arg<System.Linq.Expressions.Expression<Func<WorkflowTask, bool>>>()).ToList()));
            taskRepo.GetAsync(Arg.Any<Guid>())
                .Returns(ci => Task.FromResult(taskStore.FirstOrDefault(t => t.Id == ci.Arg<Guid>())));

            var instanceRepo = Substitute.For<IRepository<WorkflowInstance, Guid>>();

            engine = Substitute.For<IWorkflowEngine>();

            var directory = Substitute.For<IWorkflowUserDirectory>();
            directory.GetActiveDelegatorIdsAsync(currentUserId, Arg.Any<int?>())
                .Returns(Task.FromResult(delegatorIds));

            var service = new WorkflowApprovalAppService(taskRepo, instanceRepo, engine, directory);

            var session = Substitute.For<IAbpSession>();
            session.UserId.Returns(currentUserId);
            session.TenantId.Returns((int?)1);
            service.AbpSession = session;

            return service;
        }

        [Fact]
        public async Task Dado_DelegacaoAtiva_Quando_ListarPendentes_Entao_IncluiTarefasDoDelegador()
        {
            // Dado
            var tasks = new List<WorkflowTask>
            {
                new() { Id = Guid.NewGuid(), InstanceId = Guid.NewGuid(), StepId = "s1", AssigneeUserId = 7, Status = WorkflowTaskStatus.Pending },
                new() { Id = Guid.NewGuid(), InstanceId = Guid.NewGuid(), StepId = "s1", AssigneeUserId = 99, Status = WorkflowTaskStatus.Pending },
                new() { Id = Guid.NewGuid(), InstanceId = Guid.NewGuid(), StepId = "s1", AssigneeUserId = 42, Status = WorkflowTaskStatus.Pending }
            };
            var service = CreateApprovalService(tasks, currentUserId: 99, delegatorIds: new List<long> { 7 }, out _);

            // Quando
            var result = await service.GetMyPendingAsync();

            // Entao
            result.Items.Count.ShouldBe(2);
            result.Items.Select(t => t.AssigneeUserId).ShouldContain(7L);
            result.Items.Select(t => t.AssigneeUserId).ShouldContain(99L);
            result.Items.Select(t => t.AssigneeUserId).ShouldNotContain(42L);
        }

        [Fact]
        public async Task Dado_Delegado_Quando_DecidirTarefaDoDelegador_Entao_AceitaDecisao()
        {
            // Dado
            var task = new WorkflowTask { Id = Guid.NewGuid(), InstanceId = Guid.NewGuid(), StepId = "s1", AssigneeUserId = 7, Status = WorkflowTaskStatus.Pending };
            var service = CreateApprovalService(new List<WorkflowTask> { task }, currentUserId: 99, delegatorIds: new List<long> { 7 }, out var engine);

            // Quando
            await service.DecideAsync(new DecideWorkflowInput { TaskId = task.Id, Decision = "approve" });

            // Entao
            await engine.Received(1).DecideAsync(task, 99, WorkflowTaskStatus.Approved, Arg.Any<string>());
        }

        [Fact]
        public async Task Dado_SemDelegacao_Quando_DecidirTarefaAlheia_Entao_Rejeita()
        {
            // Dado
            var task = new WorkflowTask { Id = Guid.NewGuid(), InstanceId = Guid.NewGuid(), StepId = "s1", AssigneeUserId = 7, Status = WorkflowTaskStatus.Pending };
            var service = CreateApprovalService(new List<WorkflowTask> { task }, currentUserId: 99, delegatorIds: new List<long>(), out var engine);

            // Quando / Entao
            await Assert.ThrowsAsync<Abp.UI.UserFriendlyException>(() =>
                service.DecideAsync(new DecideWorkflowInput { TaskId = task.Id, Decision = "approve" }));
            await engine.DidNotReceive().DecideAsync(Arg.Any<WorkflowTask>(), Arg.Any<long>(), Arg.Any<WorkflowTaskStatus>(), Arg.Any<string>());
        }

        [Fact]
        public async Task Dado_EtapaComSla_Quando_Iniciar_Entao_TarefaTemVencimento()
        {
            // Dado — fluxo com etapa de aprovação com DueTimeHours
            var engine = CreateEngineWithSlaFlow();

            // Quando
            var instance = await engine.StartAsync(PublishedVersionForSla(), 1,
                JsonDocument.Parse("{\"reason\":\"x\"}").RootElement.Clone(), null);

            // Entao
            var task = SlaTasks.Single();
            task.DueTime.ShouldNotBeNull();
            task.DueTime.Value.ShouldBeGreaterThan(Abp.Timing.Clock.Now.AddHours(23));
        }

        private List<WorkflowTask> SlaTasks => _slaTasks;
        private List<WorkflowTask> _slaTasks;
        private List<WorkflowInstance> _slaInstances;
        private List<WorkflowTimelineEntry> _slaTimeline;
        private List<WorkflowVersion> _slaVersions;

        private WorkflowEngine CreateEngineWithSlaFlow()
        {
            _slaInstances = new List<WorkflowInstance>();
            _slaTasks = new List<WorkflowTask>();
            _slaTimeline = new List<WorkflowTimelineEntry>();
            _slaVersions = new List<WorkflowVersion>();
            var directory = Substitute.For<IWorkflowUserDirectory>();

            var c = LocalIocManager.IocContainer;
            c.Register(Castle.MicroKernel.Registration.Component.For<IRepository<WorkflowInstance, Guid>>().Instance(FakeRepo(_slaInstances)).IsDefault().Named("sla_i"));
            c.Register(Castle.MicroKernel.Registration.Component.For<IRepository<WorkflowTask, Guid>>().Instance(FakeRepo(_slaTasks)).IsDefault().Named("sla_t"));
            c.Register(Castle.MicroKernel.Registration.Component.For<IRepository<WorkflowVersion, Guid>>().Instance(FakeRepo(_slaVersions)).IsDefault().Named("sla_v"));
            var timelineRepo = Substitute.For<IRepository<WorkflowTimelineEntry, long>>();
            timelineRepo.InsertAsync(Arg.Any<WorkflowTimelineEntry>())
                .Returns(ci =>
                {
                    _slaTimeline.Add(ci.Arg<WorkflowTimelineEntry>());
                    return Task.FromResult(ci.Arg<WorkflowTimelineEntry>());
                });
            c.Register(Castle.MicroKernel.Registration.Component.For<IRepository<WorkflowTimelineEntry, long>>().Instance(timelineRepo).IsDefault().Named("sla_tl"));
            c.Register(Castle.MicroKernel.Registration.Component.For<IWorkflowUserDirectory>().Instance(directory).IsDefault().Named("sla_dir"));

            return LocalIocManager.Resolve<WorkflowEngine>();
        }

        private WorkflowVersion PublishedVersionForSla()
        {
            var flow = new WorkflowFlowModel
            {
                StartStepId = "approve",
                Steps =
                {
                    new WorkflowStepModel
                    {
                        Id = "approve", Name = "Approve", Type = "approval", NextStepId = "end",
                        Assignee = new WorkflowAssigneeModel { Kind = "users", Ids = { "1" } },
                        DueTimeHours = 24
                    },
                    new WorkflowStepModel { Id = "end", Name = "End", Type = "end" }
                }
            };
            var v = new WorkflowVersion
            {
                Id = Guid.NewGuid(),
                DefinitionId = Guid.NewGuid(),
                Version = 1,
                FormJson = "{}",
                FlowJson = JsonSerializer.Serialize(flow),
                TenantId = null
            };
            _slaVersions.Add(v);
            return v;
        }
    }
}
