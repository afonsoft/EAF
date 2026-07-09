using Abp;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Organizations;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.Users
{
    /// <summary>
    /// Testes BDD para UserStore seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class UserStoreBddTests
    {
        private static UserStore CriarUserStore(IRepository<User, long> userRepository)
        {
            return new UserStore(
                userRepository,
                Substitute.For<IRepository<UserLogin, long>>(),
                Substitute.For<IRepository<UserRole, long>>(),
                Substitute.For<IRepository<Role>>(),
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<IRepository<UserClaim, long>>(),
                Substitute.For<IRepository<UserPermissionSetting, long>>(),
                Substitute.For<IRepository<UserOrganizationUnit, long>>(),
                Substitute.For<IRepository<OrganizationUnitRole, long>>(),
                Substitute.For<IRepository<UserToken, long>>()
            );
        }

        [Fact]
        public void Dado_UsuarioExistente_Quando_GetUserById_Entao_DeveRetornarUsuario()
        {
            // Dado
            var user = new User { Id = 1, UserName = "user1" };
            var userRepository = Substitute.For<IRepository<User, long>>();
            userRepository.GetAll().Returns(new List<User> { user }.AsQueryable());

            var sut = CriarUserStore(userRepository);

            // Quando
            var result = sut.GetUserById(1);

            // Então
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
        }

        [Fact]
        public void Dado_UsuarioInexistente_Quando_GetUserById_Entao_DeveLancarExcecao()
        {
            // Dado
            var userRepository = Substitute.For<IRepository<User, long>>();
            userRepository.GetAll().Returns(new List<User>().AsQueryable());

            var sut = CriarUserStore(userRepository);

            // Quando/Então
            var exception = Should.Throw<AbpException>(() => sut.GetUserById(99));
            exception.Message.ShouldContain("There is no user");
        }
    }
}
