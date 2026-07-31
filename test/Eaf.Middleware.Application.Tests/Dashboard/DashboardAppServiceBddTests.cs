using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Dashboard;
using Eaf.Middleware.Dashboard.Dto;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Dashboard
{
    /// <summary>
    /// Testes BDD para DashboardAppService.
    /// </summary>
    public class DashboardAppServiceBddTests
    {
        private readonly DashboardAppService _sut;
        private readonly IRepository<Eaf.Middleware.MultiTenancy.Tenant, int> _tenantRepository;
        private readonly IRepository<Eaf.Middleware.Authorization.Users.User, long> _userRepository;
        private readonly IRepository<Abp.Application.Editions.Edition, int> _editionRepository;

        public DashboardAppServiceBddTests()
        {
            _tenantRepository = Substitute.For<IRepository<Eaf.Middleware.MultiTenancy.Tenant, int>>();
            _userRepository = Substitute.For<IRepository<Eaf.Middleware.Authorization.Users.User, long>>();
            _editionRepository = Substitute.For<IRepository<Abp.Application.Editions.Edition, int>>();

            _sut = new DashboardAppService(_tenantRepository, _userRepository, _editionRepository);
            _sut.UnitOfWorkManager = ManagerTestHelper.CreateUnitOfWorkManager();
        }

        [Fact]
        public async Task Dado_TenantsUsuariosEdicoes_Quando_GetHostDashboard_Entao_DeveRetornarTresTilesComContadores()
        {
            // Dado
            _tenantRepository.CountAsync().Returns(5);
            _userRepository.CountAsync().Returns(42);
            _editionRepository.CountAsync().Returns(3);

            // Quando
            var result = await _sut.GetHostDashboardAsync();

            // Então
            result.ShouldNotBeNull();
            result.IsHostDashboard.ShouldBeTrue();
            result.Tiles.Count.ShouldBe(3);
            result.Tiles.ShouldContain(t => t.Id == "totalTenants" && t.Count == 5);
            result.Tiles.ShouldContain(t => t.Id == "totalUsers" && t.Count == 42);
            result.Tiles.ShouldContain(t => t.Id == "totalEditions" && t.Count == 3);
        }

        [Fact]
        public async Task Dado_UsuarioEmTenant_Quando_GetTenantDashboard_Entao_DeveRetornarApenasTotalUsers()
        {
            // Dado
            _userRepository.CountAsync().Returns(7);
            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)1);
            _sut.AbpSession = abpSession;

            // Quando
            var result = await _sut.GetTenantDashboardAsync();

            // Então
            result.ShouldNotBeNull();
            result.IsHostDashboard.ShouldBeFalse();
            result.Tiles.Count.ShouldBe(1);
            result.Tiles[0].Id.ShouldBe("totalUsers");
            result.Tiles[0].Count.ShouldBe(7);
        }
    }
}
