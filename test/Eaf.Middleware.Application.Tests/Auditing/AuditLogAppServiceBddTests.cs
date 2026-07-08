using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Eaf.Middleware.Auditing;
using Eaf.Middleware.Auditing.Exporting;
using Eaf.Middleware.Authorization.Users;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
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
            _userRepository = Substitute.For<IRepository<User, long>>();
            _auditLogListExcelExporter = Substitute.For<IAuditLogListExcelExporter>();
            _namespaceStripper = Substitute.For<INamespaceStripper>();
            _entityChangeRepository = Substitute.For<IRepository<EntityChange, long>>();
            _entityChangeSetRepository = Substitute.For<IRepository<EntityChangeSet, long>>();
            _entityPropertyChangeRepository = Substitute.For<IRepository<EntityPropertyChange, long>>();
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
    }
}
