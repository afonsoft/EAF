using Eaf.Middleware.Application.Tests.Helpers;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Users;
using NSubstitute;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization
{
    /// <summary>
    /// Testes BDD para PermissionChecker seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class PermissionCheckerBddTests
    {
        [Fact]
        public async Task Dado_UsuarioComPermissao_Quando_IsGrantedAsync_Entao_DeveRetornarTrue()
        {
            // Dado
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.IsGrantedAsync(1, "Pages.Administration").Returns(Task.FromResult(true));

            var sut = new PermissionChecker(userManager);

            // Quando
            var result = await sut.IsGrantedAsync(1, "Pages.Administration");

            // Então
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task Dado_UsuarioSemPermissao_Quando_IsGrantedAsync_Entao_DeveRetornarFalse()
        {
            // Dado
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.IsGrantedAsync(1, "Pages.Administration").Returns(Task.FromResult(false));

            var sut = new PermissionChecker(userManager);

            // Quando
            var result = await sut.IsGrantedAsync(1, "Pages.Administration");

            // Então
            result.ShouldBeFalse();
        }

        [Fact]
        public void Dado_UsuarioComPermissao_Quando_IsGranted_Entao_DeveRetornarTrue()
        {
            // Dado
            var userManager = ManagerTestHelper.CreateUserManager();
            userManager.IsGranted(1, "Pages.Administration").Returns(true);

            var sut = new PermissionChecker(userManager);

            // Quando
            var result = sut.IsGranted(1, "Pages.Administration");

            // Então
            result.ShouldBeTrue();
        }
    }
}
