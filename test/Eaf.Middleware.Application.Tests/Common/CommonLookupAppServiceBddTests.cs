using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Common;
using Eaf.Middleware.Common.Dto;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Common
{
    public class CommonLookupAppServiceBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new CommonLookupAppService();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public async Task Dado_UsuariosCadastrados_Quando_FindUsers_Entao_DeveRetornarListaPaginada()
        {
            // Dado
            var user = new User { Id = 1, Name = "Admin", Surname = "User", UserName = "admin", EmailAddress = "admin@eaf.com" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.Users.Returns(new List<User> { user }.AsAsyncQueryable());

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns((int?)null);

            var sut = new CommonLookupAppService();
            sut.AbpSession = abpSession;
            sut.UserManager = userManager;
            sut.UnitOfWorkManager = unitOfWorkManager;

            // Quando
            var result = await sut.FindUsers(new FindUsersInput { Filter = "admin", MaxResultCount = 10, SkipCount = 0 });

            // Então
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(1);
            result.Items.Count.ShouldBe(1);
            result.Items.First().Name.ShouldContain("Admin");
        }

        [Fact]
        public async Task Dado_UsuariosCadastradosEmTenant_Quando_FindUsers_Entao_DeveRetornarListaPaginadaComTenantId()
        {
            // Dado
            var user = new User { Id = 1, Name = "Admin", Surname = "User", UserName = "admin", EmailAddress = "admin@eaf.com" };
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.Users.Returns(new List<User> { user }.AsAsyncQueryable());

            var activeUow = Substitute.For<IActiveUnitOfWork>();
            activeUow.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            unitOfWorkManager.Current.Returns(activeUow);

            var abpSession = Substitute.For<IAbpSession>();
            abpSession.TenantId.Returns(1);

            var sut = new CommonLookupAppService();
            sut.AbpSession = abpSession;
            sut.UserManager = userManager;
            sut.UnitOfWorkManager = unitOfWorkManager;

            // Quando
            var result = await sut.FindUsers(new FindUsersInput { Filter = "admin", MaxResultCount = 10, SkipCount = 0 });

            // Então
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBe(1);
            result.Items.Count.ShouldBe(1);
            result.Items.First().Name.ShouldContain("Admin");
        }
    }
}
