using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.ObjectMapping;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Dto;
using Eaf.Middleware.Editions.Dto;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.MultiTenancy.Dto;
using Eaf.Middleware.Url;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.MultiTenancy
{
    /// <summary>
    /// Testes BDD para TenantAppService seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class TenantAppServiceBddTests
    {
        private readonly TenantAppService _sut;

        public TenantAppServiceBddTests()
        {
            _sut = new TenantAppService();
        }

        #region Construtor

        [Fact]
        public void Dado_NenhumParametro_Quando_CriarInstancia_Entao_DeveInicializarPadroes()
        {
            var sut = new TenantAppService();
            sut.ShouldNotBeNull();
            sut.AppUrlService.ShouldNotBeNull();
            sut.EventBus.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_NenhumParametro_Quando_CriarInstancia_Entao_AppUrlServiceDeveSerNullInstance()
        {
            var sut = new TenantAppService();
            sut.AppUrlService.ShouldBe(NullAppUrlService.Instance);
        }

        [Fact]
        public void Dado_NenhumParametro_Quando_CriarInstancia_Entao_EventBusDeveSerNullInstance()
        {
            var sut = new TenantAppService();
            sut.EventBus.ShouldBe(NullEventBus.Instance);
        }

        #endregion

        #region Injecao de Propriedade

        [Fact]
        public void Dado_AppUrlServiceCustom_Quando_Atribuir_Entao_DeveSubstituirPadrao()
        {
            var customService = Substitute.For<IAppUrlService>();
            _sut.AppUrlService = customService;
            _sut.AppUrlService.ShouldBe(customService);
        }

        [Fact]
        public void Dado_EventBusCustom_Quando_Atribuir_Entao_DeveSubstituirPadrao()
        {
            var customEventBus = Substitute.For<IEventBus>();
            _sut.EventBus = customEventBus;
            _sut.EventBus.ShouldBe(customEventBus);
        }

        #endregion

        #region GetTenants

        [Fact]
        public async Task Dado_TenantsCadastrados_Quando_GetTenants_Entao_DeveRetornarResultadoPaginado()
        {
            // Dado
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            var tenant = new Tenant("tenant1", "Tenant One") { Id = 1, IsActive = true };
            tenantManager.Tenants.Returns(new List<Tenant> { tenant }.AsAsyncQueryable());

            _sut.TenantManager = tenantManager;
            _sut.ObjectMapper = CreateObjectMapper();

            var input = new GetTenantsInput { MaxResultCount = 10, SkipCount = 0, Sorting = "TenancyName" };

            // Quando
            var result = await _sut.GetTenants(input);

            // Então
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(1);
            result.Items.Count.ShouldBe(1);
        }

        #endregion

        #region GetTenantForEdit

        [Fact]
        public async Task Dado_TenantExistente_Quando_GetTenantForEdit_Entao_DeveRetornarTenantEditDto()
        {
            // Dado
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            var tenant = new Tenant("tenant1", "Tenant One") { Id = 1, IsActive = true };
            tenantManager.GetByIdAsync(1).Returns(tenant);

            _sut.TenantManager = tenantManager;
            _sut.ObjectMapper = CreateObjectMapper();

            // Quando
            var result = await _sut.GetTenantForEdit(new EntityDto(1));

            // Então
            result.ShouldNotBeNull();
        }

        #endregion

        #region UpdateTenant

        [Fact]
        public async Task Dado_TenantExistente_Quando_UpdateTenant_Entao_DeveAtualizarTenant()
        {
            // Dado
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            var tenant = new Tenant("tenant1", "Tenant One") { Id = 1, IsActive = true };
            tenantManager.GetByIdAsync(1).Returns(tenant);
            tenantManager.UpdateAsync(tenant).Returns(Task.CompletedTask);

            _sut.TenantManager = tenantManager;
            _sut.ObjectMapper = CreateObjectMapper();

            // Quando
            await _sut.UpdateTenant(new TenantEditDto { Id = 1, TenancyName = "tenant1", Name = "Tenant Updated", IsActive = true });

            // Então
            await tenantManager.Received(1).UpdateAsync(tenant);
        }

        #endregion

        #region DeleteTenant

        [Fact]
        public async Task Dado_TenantExistente_Quando_DeleteTenant_Entao_DeveDeletarTenant()
        {
            // Dado
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            var tenant = new Tenant("tenant1", "Tenant One") { Id = 1, IsActive = true };
            tenantManager.GetByIdAsync(1).Returns(tenant);
            tenantManager.DeleteAsync(tenant).Returns(Task.CompletedTask);

            _sut.TenantManager = tenantManager;

            // Quando
            await _sut.DeleteTenant(new EntityDto(1));

            // Então
            await tenantManager.Received(1).DeleteAsync(tenant);
        }

        #endregion

        #region GetTenantFeaturesForEdit

        [Fact]
        public async Task Dado_FeaturesDisponiveis_Quando_GetTenantFeaturesForEdit_Entao_DeveRetornarFeaturesEValores()
        {
            // Dado
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            tenantManager.GetFeatureValuesAsync(1).Returns(new List<NameValue>());

            var featureManager = Substitute.For<IFeatureManager>();
            featureManager.GetAll().Returns(new List<Feature>());

            _sut.TenantManager = tenantManager;
            _sut.FeatureManager = featureManager;
            _sut.ObjectMapper = CreateObjectMapper();

            // Quando
            var result = await _sut.GetTenantFeaturesForEdit(new EntityDto(1));

            // Então
            result.ShouldNotBeNull();
            result.Features.ShouldNotBeNull();
            result.FeatureValues.ShouldNotBeNull();
        }

        #endregion

        #region UpdateTenantFeatures

        [Fact]
        public async Task Dado_FeaturesPreenchidas_Quando_UpdateTenantFeatures_Entao_DeveChamarSetFeatureValuesAsync()
        {
            // Dado
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            tenantManager.SetFeatureValuesAsync(Arg.Any<int>(), Arg.Any<NameValue[]>()).Returns(Task.CompletedTask);

            _sut.TenantManager = tenantManager;

            // Quando
            await _sut.UpdateTenantFeatures(new UpdateTenantFeaturesInput
            {
                Id = 1,
                FeatureValues = new List<NameValueDto> { new NameValueDto(new NameValue("Feature", "Value")) }
            });

            // Então
            await tenantManager.Received(1).SetFeatureValuesAsync(1, Arg.Any<NameValue[]>());
        }

        #endregion

        #region ResetTenantSpecificFeatures

        [Fact]
        public async Task Dado_TenantId_Quando_ResetTenantSpecificFeatures_Entao_DeveChamarResetAllFeaturesAsync()
        {
            // Dado
            var tenantManager = ManagerTestHelper.CreateTenantManager();
            tenantManager.ResetAllFeaturesAsync(1).Returns(Task.CompletedTask);

            _sut.TenantManager = tenantManager;

            // Quando
            await _sut.ResetTenantSpecificFeatures(new EntityDto(1));

            // Então
            await tenantManager.Received(1).ResetAllFeaturesAsync(1);
        }

        #endregion

        #region UnlockTenantAdmin

        [Fact]
        public async Task Dado_TenantBloqueado_Quando_UnlockTenantAdmin_Entao_DeveDesbloquearAdmin()
        {
            // Dado
            var user = new User { Id = 1, UserName = "admin" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.FindByNameAsync("admin").Returns(user);

            var unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(unitOfWork);

            _sut.UserManager = userManager;
            _sut.UnitOfWorkManager = unitOfWorkManager;

            // Quando
            await _sut.UnlockTenantAdmin(new EntityDto(1));

            // Então
            user.AccessFailedCount.ShouldBe(0);
        }

        #endregion

        private IObjectMapper CreateObjectMapper()
        {
            var objectMapper = Substitute.For<IObjectMapper>();
            objectMapper.Map<List<TenantListDto>>(Arg.Any<object>()).Returns(ci =>
            {
                var source = ci.Arg<object>();
                var count = source is System.Collections.IEnumerable e ? e.Cast<object>().Count() : 1;
                var list = new List<TenantListDto>();
                for (int i = 0; i < count; i++)
                {
                    list.Add(new TenantListDto());
                }
                return list;
            });
            objectMapper.Map<TenantEditDto>(Arg.Any<object>()).Returns(new TenantEditDto());
            objectMapper.Map<TenantEditDto, Tenant>(Arg.Any<TenantEditDto>(), Arg.Any<Tenant>()).Returns(tenant => tenant.Arg<Tenant>());
            objectMapper.Map<List<FlatFeatureDto>>(Arg.Any<object>()).Returns(new List<FlatFeatureDto>());
            return objectMapper;
        }
    }
}
