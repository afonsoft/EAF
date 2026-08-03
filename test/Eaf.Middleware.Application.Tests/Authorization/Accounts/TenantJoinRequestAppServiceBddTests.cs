using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Abp.UI;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Accounts;
using Eaf.Middleware.Authorization.Accounts.Dto;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Accounts
{
    /// <summary>
    /// Testes BDD para TenantJoinRequestAppService.
    /// </summary>
    public class TenantJoinRequestAppServiceBddTests
    {
        private readonly TenantJoinRequestAppService _sut;
        private readonly IRepository<TenantJoinRequest, long> _joinRequestRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly ITenantUserManager _tenantUserManager;
        private readonly ISettingManager _settingManager;
        private readonly IObjectMapper _objectMapper;

        public TenantJoinRequestAppServiceBddTests()
        {
            _joinRequestRepository = Substitute.For<IRepository<TenantJoinRequest, long>>();
            _tenantRepository = Substitute.For<IRepository<Tenant>>();
            _userRepository = Substitute.For<IRepository<User, long>>();
            _tenantUserManager = Substitute.For<ITenantUserManager>();
            _settingManager = Substitute.For<ISettingManager>();
            _objectMapper = CreateObjectMapper();

            _tenantRepository.GetAll().Returns(new List<Tenant>().AsAsyncQueryable());
            _userRepository.GetAll().Returns(new List<User>().AsAsyncQueryable());

            _sut = new TenantJoinRequestAppService(
                _joinRequestRepository,
                _tenantRepository,
                _userRepository,
                _tenantUserManager);

            _sut.SettingManager = _settingManager;
            _sut.ObjectMapper = _objectMapper;
            _sut.UnitOfWorkManager = ManagerTestHelper.CreateUnitOfWorkManager();
            _sut.LocalizationManager = Substitute.For<Abp.Localization.ILocalizationManager>();
        }

        [Fact]
        public async Task Dado_JoinRequestsDesabilitados_Quando_CriarSolicitacao_Entao_DeveLancarExcecao()
        {
            // Dado
            ConfigurarSetting(AppSettings.TenantManagement.AllowJoinRequests, false);
            ConfigurarSessao(10, null);

            // Quando / Então
            await Should.ThrowAsync<UserFriendlyException>(async () =>
                await _sut.CreateRequestAsync(new CreateTenantJoinRequestInput { TenantId = 1, Message = "Quero entrar" }));
        }

        [Fact]
        public async Task Dado_JoinRequestsHabilitados_Quando_CriarSolicitacao_Entao_DeveRetornarDto()
        {
            // Dado
            ConfigurarSetting(AppSettings.TenantManagement.AllowJoinRequests, true);
            ConfigurarSessao(10, null);

            var request = new TenantJoinRequest { Id = 100, UserId = 10, TenantId = 1, Status = TenantJoinRequestStatus.Pending, Message = "Quero entrar" };
            _tenantUserManager.CreatePendingMembershipAsync(10, 1, "Quero entrar").Returns(request);

            // Quando
            var result = await _sut.CreateRequestAsync(new CreateTenantJoinRequestInput { TenantId = 1, Message = "Quero entrar" });

            // Então
            result.ShouldNotBeNull();
            result.Id.ShouldBe(100);
            await _tenantUserManager.Received(1).CreatePendingMembershipAsync(10, 1, "Quero entrar");
        }

        [Fact]
        public async Task Dado_UsuarioLogado_Quando_ListarMinhasSolicitacoes_Entao_DeveRetornarLista()
        {
            // Dado
            ConfigurarSessao(10, null);
            var requests = new List<TenantJoinRequest>
            {
                new TenantJoinRequest { Id = 1, UserId = 10, TenantId = 1, Status = TenantJoinRequestStatus.Pending }
            };
            _joinRequestRepository.GetAll().Returns(requests.AsAsyncQueryable());

            // Quando
            var result = await _sut.GetMyRequestsAsync();

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Dado_TenantAtual_Quando_ListarPendentes_Entao_DeveRetornarLista()
        {
            // Dado
            ConfigurarSessao(5, 1);
            var requests = new List<TenantJoinRequest>
            {
                new TenantJoinRequest { Id = 1, UserId = 10, TenantId = 1, Status = TenantJoinRequestStatus.Pending }
            };
            _joinRequestRepository.GetAll().Returns(requests.AsAsyncQueryable());

            // Quando
            var result = await _sut.GetPendingRequestsForCurrentTenantAsync();

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Dado_UsuarioHostSemTenant_Quando_ListarPendentes_Entao_DeveRetornarListaVazia()
        {
            // Dado
            ConfigurarSessao(5, null);

            // Quando
            var result = await _sut.GetPendingRequestsForCurrentTenantAsync();

            // Então
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Dado_SolicitacaoPendente_Quando_Aprovar_Entao_DeveChamarTenantUserManager()
        {
            // Dado
            ConfigurarSessao(99, 1);

            // Quando
            await _sut.ApproveAsync(new ApproveTenantJoinRequestInput { RequestId = 100, IsApproved = true });

            // Então
            await _tenantUserManager.Received(1).ApproveMembershipAsync(100, 99);
        }

        [Fact]
        public async Task Dado_SolicitacaoPendente_Quando_Rejeitar_Entao_DeveAtualizarStatus()
        {
            // Dado
            ConfigurarSessao(99, 1);
            var request = new TenantJoinRequest { Id = 100, UserId = 10, TenantId = 1, Status = TenantJoinRequestStatus.Pending };
            _joinRequestRepository.GetAsync(100).Returns(request);

            // Quando
            await _sut.ApproveAsync(new ApproveTenantJoinRequestInput { RequestId = 100, IsApproved = false });

            // Então
            request.Status.ShouldBe(TenantJoinRequestStatus.Rejected);
            request.ApproverUserId.ShouldBe(99);
        }

        private void ConfigurarSetting(string nome, bool valor)
        {
            _settingManager.GetSettingValueAsync(nome).Returns(Task.FromResult(valor.ToString().ToLowerInvariant()));
        }

        private void ConfigurarSessao(long userId, int? tenantId)
        {
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(userId);
            abpSession.UserId.Value.Returns(userId);
            abpSession.TenantId.Returns(tenantId);
            if (tenantId.HasValue)
                abpSession.TenantId.Value.Returns(tenantId.Value);
            _sut.AbpSession = abpSession;
        }

        private IObjectMapper CreateObjectMapper()
        {
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<AvailableTenantDto>>(Arg.Any<object>()).Returns(ci =>
            {
                var source = ci.Arg<object>();
                var count = source is System.Collections.IEnumerable e ? e.Cast<object>().Count() : 1;
                var list = new List<AvailableTenantDto>();
                for (int i = 0; i < count; i++)
                    list.Add(new AvailableTenantDto());
                return list;
            });
            objectMapper.Map<List<TenantJoinRequestDto>>(Arg.Any<List<TenantJoinRequest>>()).Returns(ci =>
            {
                var source = ci.Arg<List<TenantJoinRequest>>();
                var list = new List<TenantJoinRequestDto>();
                foreach (var r in source)
                    list.Add(new TenantJoinRequestDto
                    {
                        Id = r.Id,
                        UserId = r.UserId,
                        TenantId = r.TenantId,
                        Status = r.Status,
                        Message = r.Message
                    });
                return list;
            });
            return objectMapper;
        }
    }
}
