using Abp.Domain.Repositories;
using Abp.UI;
using Castle.MicroKernel.Registration;
using Eaf.Workflow.Domain;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Workflow.Tests
{
    /// <summary>
    /// Testes do motor de workflow (Phase A): validação, start, quorum, send-back, version pinning, simulação.
    /// </summary>
    public class WorkflowEngineTests : EafWorkflowTestBase
    {
        private const string FormJson = "{\"fields\":[{\"id\":\"reason\",\"label\":\"Reason\",\"type\":\"text\",\"required\":true}]}";
        private const string FlowJson = "{\"startStepId\":\"s1\",\"steps\":[" +
            "{\"id\":\"s1\",\"type\":\"approval\",\"assignee\":{\"kind\":\"users\",\"ids\":[\"10\",\"11\"],\"quorum\":\"any\"},\"nextStepId\":\"s2\"}," +
            "{\"id\":\"s2\",\"type\":\"end\"}]}";

        private List<WorkflowInstance> _instances;
        private List<WorkflowTask> _tasks;
        private List<WorkflowTimelineEntry> _timeline;
        private List<WorkflowVersion> _versions;
        private IWorkflowUserDirectory _directory;
        private WorkflowEngine _engine;

        private WorkflowEngine CreateEngine()
        {
            _instances = new List<WorkflowInstance>();
            _tasks = new List<WorkflowTask>();
            _timeline = new List<WorkflowTimelineEntry>();
            _versions = new List<WorkflowVersion>();
            _directory = Substitute.For<IWorkflowUserDirectory>();

            var c = LocalIocManager.IocContainer;
            c.Register(Component.For<IRepository<WorkflowInstance, Guid>>().Instance(FakeRepo(_instances)).IsDefault().Named("wf_i"));
            c.Register(Component.For<IRepository<WorkflowTask, Guid>>().Instance(FakeRepo(_tasks)).IsDefault().Named("wf_t"));
            c.Register(Component.For<IRepository<WorkflowTimelineEntry, long>>().Instance(FakeTimelineRepo(_timeline)).IsDefault().Named("wf_tl"));
            c.Register(Component.For<IRepository<WorkflowVersion, Guid>>().Instance(FakeRepo(_versions)).IsDefault().Named("wf_v"));
            c.Register(Component.For<IWorkflowUserDirectory>().Instance(_directory).IsDefault().Named("wf_dir"));

            _engine = LocalIocManager.Resolve<WorkflowEngine>();
            return _engine;
        }

        private static IRepository<WorkflowTimelineEntry, long> FakeTimelineRepo(List<WorkflowTimelineEntry> store)
        {
            var repo = Substitute.For<IRepository<WorkflowTimelineEntry, long>>();
            repo.InsertAsync(Arg.Any<WorkflowTimelineEntry>())
                .Returns(ci =>
                {
                    store.Add(ci.Arg<WorkflowTimelineEntry>());
                    return Task.FromResult(ci.Arg<WorkflowTimelineEntry>());
                });
            repo.GetAllListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<WorkflowTimelineEntry, bool>>>())
                .Returns(ci => Task.FromResult(store.FindAll(ci.Arg<System.Linq.Expressions.Expression<Func<WorkflowTimelineEntry, bool>>>().Compile().Invoke)));
            return repo;
        }

        private WorkflowVersion PublishedVersion(int version = 1, string flowJson = FlowJson)
        {
            var v = new WorkflowVersion
            {
                Id = Guid.NewGuid(),
                DefinitionId = Guid.NewGuid(),
                Version = version,
                FormJson = FormJson,
                FlowJson = flowJson,
                TenantId = null
            };
            _versions.Add(v);
            return v;
        }

        private static JsonElement Values(string json = "{\"reason\":\"vacation\"}")
            => JsonDocument.Parse(json).RootElement.Clone();

        [Fact]
        public async Task Dado_VersaoPublicada_Quando_Iniciar_Entao_CriaInstanciaETarefas()
        {
            // Dado
            var engine = CreateEngine();
            var version = PublishedVersion();

            // Quando
            var instance = await engine.StartAsync(version, 42, Values(), null);

            // Então
            instance.Status.ShouldBe(WorkflowInstanceStatus.InProgress);
            instance.CurrentStepId.ShouldBe("s1");
            _tasks.Count.ShouldBe(2);
            _timeline.ShouldContain(e => e.EventType == "submitted");
            _timeline.ShouldContain(e => e.EventType == "activated");
        }

        [Fact]
        public async Task Dado_QuorumAny_Quando_UmAprova_Entao_Completa()
        {
            // Dado
            var engine = CreateEngine();
            var instance = await engine.StartAsync(PublishedVersion(), 42, Values(), null);
            var task = _tasks[0];

            // Quando
            var result = await engine.DecideAsync(task, 10, WorkflowTaskStatus.Approved, "ok");

            // Então
            result.Status.ShouldBe(WorkflowInstanceStatus.Completed);
            _tasks[1].Status.ShouldBe(WorkflowTaskStatus.Cancelled);
            _timeline.ShouldContain(e => e.EventType == "completed");
        }

        [Fact]
        public async Task Dado_QuorumAll_Quando_UmAprova_Entao_AguardaSegundo()
        {
            // Dado
            var flow = FlowJson.Replace("\"quorum\":\"any\"", "\"quorum\":\"all\"");
            var engine = CreateEngine();
            var instance = await engine.StartAsync(PublishedVersion(flowJson: flow), 42, Values(), null);

            // Quando — primeiro aprova
            var mid = await engine.DecideAsync(_tasks[0], 10, WorkflowTaskStatus.Approved, "ok");
            mid.Status.ShouldBe(WorkflowInstanceStatus.InProgress);
            _tasks[1].Status.ShouldBe(WorkflowTaskStatus.Pending);

            // Quando — segundo aprova
            var done = await engine.DecideAsync(_tasks[1], 11, WorkflowTaskStatus.Approved, "ok");
            done.Status.ShouldBe(WorkflowInstanceStatus.Completed);
        }

        [Fact]
        public async Task Dado_SendBack_Quando_Decisao_Entao_RetornaAoRequester()
        {
            // Dado
            var engine = CreateEngine();
            var instance = await engine.StartAsync(PublishedVersion(), 42, Values(), null);

            // Quando
            var result = await engine.DecideAsync(_tasks[0], 10, WorkflowTaskStatus.SentBack, "incomplete");

            // Então (BR-003)
            result.Status.ShouldBe(WorkflowInstanceStatus.SentBack);
            result.FormValuesJson.ShouldContain("vacation");
            _timeline.ShouldContain(e => e.EventType == "sent-back");
        }

        [Fact]
        public async Task Dado_TarefaJaDecidida_Quando_DecidirDeNovo_Entao_Idempotente()
        {
            // Dado
            var engine = CreateEngine();
            var instance = await engine.StartAsync(PublishedVersion(), 42, Values(), null);
            await engine.DecideAsync(_tasks[0], 10, WorkflowTaskStatus.Approved, "ok");

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(
                () => engine.DecideAsync(_tasks[0], 10, WorkflowTaskStatus.Approved, "again"));
        }

        [Fact]
        public async Task Dado_CampoObrigatorioFaltando_Quando_Iniciar_Entao_Rejeita()
        {
            // Dado
            var engine = CreateEngine();

            // Quando / Então
            var ex = await Should.ThrowAsync<UserFriendlyException>(
                () => engine.StartAsync(PublishedVersion(), 42, Values("{}"), null));
            ex.Message.ShouldContain("Reason");
        }

        [Fact]
        public async Task Dado_InstanciaPinada_Quando_Republicado_Entao_UsaVersaoAntiga()
        {
            // Dado — instância na v1
            var engine = CreateEngine();
            var v1 = PublishedVersion(1);
            var instance = await engine.StartAsync(v1, 42, Values(), null);

            // Nova publicação v2 com grafo diferente — a instância continua na v1 (BR-002)
            var v2 = new WorkflowVersion
            {
                Id = Guid.NewGuid(), DefinitionId = v1.DefinitionId, Version = 2,
                FormJson = FormJson, FlowJson = "{\"startStepId\":\"x\",\"steps\":[{\"id\":\"x\",\"type\":\"end\"}]}"
            };
            _versions.Add(v2);

            // Quando
            var result = await engine.DecideAsync(_tasks[0], 10, WorkflowTaskStatus.Approved, "ok");

            // Então
            result.Version.ShouldBe(1);
            result.Status.ShouldBe(WorkflowInstanceStatus.Completed);
        }

        [Fact]
        public async Task Dado_FluxoComCondicao_Quando_Simular_Entao_RetornaCaminho()
        {
            // Dado
            var flow = "{\"startStepId\":\"c1\",\"steps\":[" +
                "{\"id\":\"c1\",\"type\":\"condition\",\"condition\":\"amount > 100\",\"nextStepId\":\"a1\",\"rejectStepId\":\"e\"}," +
                "{\"id\":\"a1\",\"type\":\"approval\",\"assignee\":{\"kind\":\"users\",\"ids\":[\"7\"],\"quorum\":\"any\"},\"nextStepId\":\"e\"}," +
                "{\"id\":\"e\",\"type\":\"end\"}]}";
            var engine = CreateEngine();
            var version = PublishedVersion(flowJson: flow);

            // Quando — amount alto → passa por aprovação
            var simHigh = await engine.SimulateAsync(version, Values("{\"reason\":\"x\",\"amount\":\"500\"}"), 42);
            simHigh.Path.ShouldBe(new List<string> { "c1", "a1", "e" });
            simHigh.Assignees["a1"].ShouldBe(new List<long> { 7 });

            // Quando — amount baixo → direto pro fim
            var simLow = await engine.SimulateAsync(version, Values("{\"reason\":\"x\",\"amount\":\"50\"}"), 42);
            simLow.Path.ShouldBe(new List<string> { "c1", "e" });
        }

        [Fact]
        public async Task Dado_AssigneePorRole_Quando_Iniciar_Entao_ResolveViaDiretorio()
        {
            // Dado
            var flow = "{\"startStepId\":\"s1\",\"steps\":[" +
                "{\"id\":\"s1\",\"type\":\"approval\",\"assignee\":{\"kind\":\"role\",\"ids\":[\"Managers\"],\"quorum\":\"any\"},\"nextStepId\":\"s2\"}," +
                "{\"id\":\"s2\",\"type\":\"end\"}]}";
            var engine = CreateEngine();
            _directory.GetUsersInRoleAsync("Managers", Arg.Any<int?>()).Returns(Task.FromResult(new List<long> { 55, 66 }));

            // Quando
            var instance = await engine.StartAsync(PublishedVersion(flowJson: flow), 42, Values(), null);

            // Então
            _tasks.Count.ShouldBe(2);
            _tasks.ShouldContain(t => t.AssigneeUserId == 55);
            _tasks.ShouldContain(t => t.AssigneeUserId == 66);
        }

        [Fact]
        public async Task Dado_Resubmit_Quando_Devolvido_Entao_ReativaEtapa()
        {
            // Dado
            var engine = CreateEngine();
            var instance = await engine.StartAsync(PublishedVersion(), 42, Values(), null);
            await engine.DecideAsync(_tasks[0], 10, WorkflowTaskStatus.SentBack, "fix it");

            // Quando
            await engine.ResubmitAsync(instance, Values("{\"reason\":\"vacation v2\"}"), 42);

            // Então
            instance.Status.ShouldBe(WorkflowInstanceStatus.InProgress);
            _tasks.FindAll(t => t.Status == WorkflowTaskStatus.Pending).Count.ShouldBe(2);
        }
    }
}
