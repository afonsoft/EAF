using Abp;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.MultiTenancy;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests
{
    /// <summary>
    /// Testes BDD para MiddlewareAppServiceBase seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class MiddlewareAppServiceBaseBddTests
    {
        private class TestMiddlewareAppService : MiddlewareAppServiceBase
        {
            public Task<User> PublicGetCurrentUserAsync() => GetCurrentUserAsync();
            public User PublicGetCurrentUser() => GetCurrentUser();
            public Task<Tenant> PublicGetCurrentTenantAsync() => GetCurrentTenantAsync();
            public Tenant PublicGetCurrentTenant() => GetCurrentTenant();
        }

        private static TestMiddlewareAppService CreateSut()
        {
            var userManager = ManagerTestHelper.CreateUserManager();
            var tenantManager = ManagerTestHelper.CreateTenantManager();

            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var activeUnitOfWork = Substitute.For<IActiveUnitOfWork>();
            activeUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            unitOfWorkManager.Current.Returns(activeUnitOfWork);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.UserId.Returns(1L);
            abpSession.TenantId.Returns(1);

            return new TestMiddlewareAppService
            {
                UserManager = userManager,
                TenantManager = tenantManager,
                UnitOfWorkManager = unitOfWorkManager,
                AbpSession = abpSession
            };
        }

        [Fact]
        public async Task Dado_UsuarioLogado_Quando_GetCurrentUserAsync_Entao_DeveRetornarUsuarioAtual()
        {
            // Dado
            var currentUser = new User { Id = 1, UserName = "admin" };
            var sut = CreateSut();
            sut.UserManager.FindByIdAsync("1").Returns(currentUser);

            // Quando
            var result = await sut.PublicGetCurrentUserAsync();

            // Então
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
        }

        [Fact]
        public async Task Dado_TenantExistente_Quando_GetCurrentTenantAsync_Entao_DeveRetornarTenantAtual()
        {
            // Dado
            var currentTenant = new Tenant("tenant1", "Tenant One") { Id = 1 };
            var sut = CreateSut();
            sut.TenantManager.GetByIdAsync(1).Returns(currentTenant);

            // Quando
            var result = await sut.PublicGetCurrentTenantAsync();

            // Então
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
        }

        [Fact]
        public void Dado_UsuarioExistente_Quando_GetCurrentUser_Entao_DeveRetornarUsuarioAtual()
        {
            // Dado
            var currentUser = new User { Id = 1, UserName = "admin" };
            var sut = CreateSut();
            sut.UserManager.FindByIdAsync("1").Returns(currentUser);

            // Quando
            var result = sut.PublicGetCurrentUser();

            // Então
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
        }

        [Fact]
        public void Dado_UsuarioNaoEncontrado_Quando_GetCurrentUser_Entao_DeveLancarExcecao()
        {
            // Dado
            var sut = CreateSut();
            sut.UserManager.FindByIdAsync("1").Returns((User?)null);

            // Quando / Então
            Should.Throw<AbpException>(() => sut.PublicGetCurrentUser());
        }

    }
}
