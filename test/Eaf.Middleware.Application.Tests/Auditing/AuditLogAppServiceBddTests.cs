using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Abp.ObjectMapping;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Auditing;
using Eaf.Middleware.Auditing.Dto;
using Eaf.Middleware.Auditing.Exporting;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Dto;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Auditing
{
    /// <summary>
    /// Testes BDD para AuditLogAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class AuditLogAppServiceBddTests
    {
        private readonly IRepository<AuditLog, long> _auditLogRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IAuditLogListExcelExporter _auditLogListExcelExporter;
        private readonly INamespaceStripper _namespaceStripper;
        private readonly IRepository<EntityChange, long> _entityChangeRepository;
        private readonly IRepository<EntityChangeSet, long> _entityChangeSetRepository;
        private readonly IRepository<EntityPropertyChange, long> _entityPropertyChangeRepository;
        private readonly IAbpStartupConfiguration _eafStartupConfiguration;
        private readonly AuditLogAppService _sut;

        public AuditLogAppServiceBddTests()
        {
            _auditLogRepository = Substitute.For<IRepository<AuditLog, long>>();
            _auditLogRepository.GetAllAsync().Returns(_ => Task.FromResult(_auditLogRepository.GetAll()));
            _userRepository = Substitute.For<IRepository<User, long>>();
            _userRepository.GetAllAsync().Returns(_ => Task.FromResult(_userRepository.GetAll()));
            _auditLogListExcelExporter = Substitute.For<IAuditLogListExcelExporter>();
            _namespaceStripper = Substitute.For<INamespaceStripper>();
            _entityChangeRepository = Substitute.For<IRepository<EntityChange, long>>();
            _entityChangeRepository.GetAllAsync().Returns(_ => Task.FromResult(_entityChangeRepository.GetAll()));
            _entityChangeSetRepository = Substitute.For<IRepository<EntityChangeSet, long>>();
            _entityChangeSetRepository.GetAllAsync().Returns(_ => Task.FromResult(_entityChangeSetRepository.GetAll()));
            _entityPropertyChangeRepository = Substitute.For<IRepository<EntityPropertyChange, long>>();
            _entityPropertyChangeRepository.GetAllAsync().Returns(_ => Task.FromResult(_entityPropertyChangeRepository.GetAll()));
            _eafStartupConfiguration = Substitute.For<IAbpStartupConfiguration>();

            _sut = new AuditLogAppService(
                _auditLogRepository,
                _userRepository,
                _auditLogListExcelExporter,
                _namespaceStripper,
                _entityChangeRepository,
                _entityChangeSetRepository,
                _entityPropertyChangeRepository,
                _eafStartupConfiguration
            );

            SetupObjectMapper();
            SetupNamespaceStripper();
            SetupExcelExporter();
        }

        private void SetupObjectMapper()
        {
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<AuditLogListDto>(Arg.Any<object>())
                .Returns(x => new AuditLogListDto { ServiceName = ((AuditLog)x[0]).ServiceName });
            objectMapper.Map<EntityChangeListDto>(Arg.Any<object>())
                .Returns(x => new EntityChangeListDto());
            _sut.ObjectMapper = objectMapper;
        }

        private void SetupNamespaceStripper()
        {
            _namespaceStripper.StripNameSpace(Arg.Any<string>())
                .Returns(x => x.Arg<string>());
        }

        private void SetupExcelExporter()
        {
            _auditLogListExcelExporter.ExportToFile(Arg.Any<List<AuditLogListDto>>())
                .Returns(x => new FileDto("auditLogs.xlsx", "xlsx"));
            _auditLogListExcelExporter.ExportToFile(Arg.Any<List<EntityChangeListDto>>())
                .Returns(x => new FileDto("entityChanges.xlsx", "xlsx"));
        }

        #region Construtor

        [Fact]
        public void Dado_Dependencias_Quando_CriarInstancia_Entao_DeveSerValido()
        {
            _sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_OitoDependencias_Quando_CriarInstancia_Entao_NaoDeveLancarExcecao()
        {
            // Dado / Quando
            var sut = new AuditLogAppService(
                _auditLogRepository,
                _userRepository,
                _auditLogListExcelExporter,
                _namespaceStripper,
                _entityChangeRepository,
                _entityChangeSetRepository,
                _entityPropertyChangeRepository,
                _eafStartupConfiguration
            );

            // Então
            sut.ShouldNotBeNull();
        }

        #endregion

        #region GetEntityHistoryObjectTypes

        [Fact]
        public void Dado_ConfiguracoesCustomizadasPreenchidas_Quando_GetEntityHistoryObjectTypes_Entao_DeveRetornarListaNameValue()
        {
            // Dado
            var config = new Dictionary<string, object>
            {
                { "TipoA", "ValorA" },
                { "TipoB", 42 }
            };
            _eafStartupConfiguration.GetCustomConfig().Returns(config);

            // Quando
            var result = _sut.GetEntityHistoryObjectTypes();

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.ShouldContain(x => x.Name == "TipoA" && x.Value == "ValorA");
            result.ShouldContain(x => x.Name == "TipoB" && x.Value == "42");
        }

        [Fact]
        public void Dado_ConfiguracoesCustomizadasVazias_Quando_GetEntityHistoryObjectTypes_Entao_DeveRetornarListaVazia()
        {
            // Dado
            _eafStartupConfiguration.GetCustomConfig().Returns(new Dictionary<string, object>());

            // Quando
            var result = _sut.GetEntityHistoryObjectTypes();

            // Então
            result.ShouldBeEmpty();
        }

        #endregion

        #region AuditLogs

        [Fact]
        public async Task Dado_AuditLogsEUsuarios_Quando_GetAuditLogs_Entao_DeveRetornarResultadoPaginado()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var auditLog = new AuditLog
            {
                Id = 1,
                UserId = 1,
                ExecutionTime = new DateTime(2025, 6, 1),
                ServiceName = "Eaf.Middleware.Test.TestService",
                MethodName = "TestMethod",
                BrowserInfo = "Chrome",
                ExecutionDuration = 100
            };

            _auditLogRepository.GetAll().Returns(new List<AuditLog> { auditLog }.AsAsyncQueryable());
            _userRepository.GetAll().Returns(new List<User> { user }.AsAsyncQueryable());

            var input = new GetAuditLogsInput
            {
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31),
                MaxResultCount = 10,
                SkipCount = 0,
                Sorting = "AuditLog.ExecutionTime desc"
            };

            // Quando
            var result = await _sut.GetAuditLogs(input);

            // Então
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(1);
            result.Items.Count.ShouldBe(1);
            result.Items[0].UserName.ShouldBe("admin");
            result.Items[0].ServiceName.ShouldBe("Eaf.Middleware.Test.TestService");
        }

        [Fact]
        public async Task Dado_AuditLogsDisponiveis_Quando_GetAuditLogsToExcel_Entao_DeveRetornarArquivo()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var auditLog = new AuditLog
            {
                Id = 1,
                UserId = 1,
                ExecutionTime = new DateTime(2025, 6, 1),
                ServiceName = "Eaf.Middleware.Test.TestService",
                MethodName = "TestMethod",
                ExecutionDuration = 100
            };

            _auditLogRepository.GetAll().Returns(new List<AuditLog> { auditLog }.AsAsyncQueryable());
            _userRepository.GetAll().Returns(new List<User> { user }.AsAsyncQueryable());

            var input = new GetAuditLogsInput
            {
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31)
            };

            // Quando
            var result = await _sut.GetAuditLogsToExcel(input);

            // Então
            result.ShouldNotBeNull();
            result.FileName.ShouldBe("auditLogs.xlsx");
        }

        [Fact]
        public async Task Dado_AuditLogsComFiltros_Quando_GetAuditLogs_Entao_DeveRetornarResultadoFiltrado()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var auditLog = new AuditLog
            {
                Id = 1,
                UserId = 1,
                ExecutionTime = new DateTime(2025, 6, 1),
                ServiceName = "Eaf.Middleware.Test.TestService",
                MethodName = "TestMethod",
                BrowserInfo = "Chrome",
                ExecutionDuration = 100,
                Exception = "Error"
            };

            _auditLogRepository.GetAll().Returns(new List<AuditLog> { auditLog }.AsAsyncQueryable());
            _userRepository.GetAll().Returns(new List<User> { user }.AsAsyncQueryable());

            var input = new GetAuditLogsInput
            {
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31),
                UserName = "admin",
                ServiceName = "Test",
                MethodName = "Test",
                BrowserInfo = "Chrome",
                MinExecutionDuration = 50,
                MaxExecutionDuration = 200,
                HasException = true,
                MaxResultCount = 10,
                SkipCount = 0,
                Sorting = "AuditLog.ExecutionTime desc"
            };

            // Quando
            var result = await _sut.GetAuditLogs(input);

            // Então
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(1);
            result.Items.Count.ShouldBe(1);
        }

        #endregion

        #region EntityChanges

        [Fact]
        public async Task Dado_EntityChangesEUsuarios_Quando_GetEntityChanges_Entao_DeveRetornarResultadoPaginado()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var entityChangeSet = new EntityChangeSet { Id = 1, UserId = 1 };
            var entityChange = new EntityChange
            {
                Id = 1,
                EntityChangeSetId = 1,
                EntityTypeFullName = "Eaf.Middleware.Test.Entity",
                ChangeTime = new DateTime(2025, 6, 1)
            };

            _entityChangeSetRepository.GetAll().Returns(new List<EntityChangeSet> { entityChangeSet }.AsAsyncQueryable());
            _entityChangeRepository.GetAll().Returns(new List<EntityChange> { entityChange }.AsAsyncQueryable());
            _userRepository.GetAll().Returns(new List<User> { user }.AsAsyncQueryable());

            var input = new GetEntityChangeInput
            {
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31),
                MaxResultCount = 10,
                SkipCount = 0,
                Sorting = "EntityChange.ChangeTime desc"
            };

            // Quando
            var result = await _sut.GetEntityChanges(input);

            // Então
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(1);
            result.Items.Count.ShouldBe(1);
            result.Items[0].UserName.ShouldBe("admin");
        }

        [Fact]
        public async Task Dado_EntityPropertyChanges_Quando_GetEntityPropertyChanges_Entao_DeveRetornarListaMapeada()
        {
            // Dado
            var entityPropertyChange = new EntityPropertyChange
            {
                Id = 1,
                EntityChangeId = 10,
                PropertyName = "Name",
                OriginalValue = "Old",
                NewValue = "New",
                PropertyTypeFullName = "System.String"
            };

            _entityPropertyChangeRepository.GetAll().Returns(new List<EntityPropertyChange> { entityPropertyChange }.AsAsyncQueryable());

            // Quando
            var result = await _sut.GetEntityPropertyChanges(10);

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].PropertyName.ShouldBe("Name");
            result[0].OriginalValue.ShouldBe("Old");
            result[0].NewValue.ShouldBe("New");
        }

        [Fact]
        public async Task Dado_TipoEEntidadeEspecificos_Quando_GetEntityTypeChanges_Entao_DeveRetornarResultadoPaginado()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var entityChangeSet = new EntityChangeSet { Id = 1, UserId = 1 };
            var entityChange = new EntityChange
            {
                Id = 1,
                EntityChangeSetId = 1,
                EntityTypeFullName = "Eaf.Middleware.Test.Entity",
                EntityId = "42",
                ChangeTime = new DateTime(2025, 6, 1)
            };

            _entityChangeSetRepository.GetAll().Returns(new List<EntityChangeSet> { entityChangeSet }.AsAsyncQueryable());
            _entityChangeRepository.GetAll().Returns(new List<EntityChange> { entityChange }.AsAsyncQueryable());
            _userRepository.GetAll().Returns(new List<User> { user }.AsAsyncQueryable());

            var input = new GetEntityTypeChangeInput
            {
                EntityTypeFullName = "Eaf.Middleware.Test.Entity",
                EntityId = "42",
                MaxResultCount = 10,
                SkipCount = 0,
                Sorting = "EntityChange.ChangeTime desc"
            };

            // Quando
            var result = await _sut.GetEntityTypeChanges(input);

            // Então
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(1);
            result.Items.Count.ShouldBe(1);
            result.Items[0].UserName.ShouldBe("admin");
        }

        [Fact]
        public async Task Dado_EntityChangesEUsuarios_Quando_GetEntityChangesToExcel_Entao_DeveRetornarArquivo()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var entityChangeSet = new EntityChangeSet { Id = 1, UserId = 1 };
            var entityChange = new EntityChange
            {
                Id = 1,
                EntityChangeSetId = 1,
                EntityTypeFullName = "Eaf.Middleware.Test.Entity",
                ChangeTime = new DateTime(2025, 6, 1)
            };

            _entityChangeSetRepository.GetAll().Returns(new List<EntityChangeSet> { entityChangeSet }.AsAsyncQueryable());
            _entityChangeRepository.GetAll().Returns(new List<EntityChange> { entityChange }.AsAsyncQueryable());
            _userRepository.GetAll().Returns(new List<User> { user }.AsAsyncQueryable());

            var input = new GetEntityChangeInput
            {
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31)
            };

            // Quando
            var result = await _sut.GetEntityChangesToExcel(input);

            // Então
            result.ShouldNotBeNull();
            result.FileName.ShouldBe("entityChanges.xlsx");
        }

        #endregion
    }
}
