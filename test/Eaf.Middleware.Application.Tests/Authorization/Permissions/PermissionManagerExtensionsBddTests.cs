using System.Collections.Generic;
using System.Linq;
using Abp.Authorization;
using Abp.Runtime.Validation;
using Eaf.Middleware.Authorization.Permissions;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization
{
    /// <summary>
    /// Testes BDD para PermissionManagerExtensions seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class PermissionManagerExtensionsBddTests
    {
        [Fact]
        public void Dado_ClasseDeExtensao_Quando_VerificarTipo_Entao_DeveSerEstatica()
        {
            var tipo = typeof(PermissionManagerExtensions);
            (tipo.IsAbstract && tipo.IsSealed).ShouldBeTrue();
        }

        [Fact]
        public void Dado_ListaVaziaDeNomes_Quando_GetPermissionsFromNamesByValidating_Entao_DeveRetornarVazio()
        {
            var permissionManager = Substitute.For<IPermissionManager>();

            var result = permissionManager.GetPermissionsFromNamesByValidating(new List<string>());

            result.ShouldBeEmpty();
        }

        [Fact]
        public void Dado_NomeDePermissaoInexistente_Quando_GetPermissionsFromNamesByValidating_Entao_DeveLancarAbpValidationException()
        {
            var permissionManager = Substitute.For<IPermissionManager>();
            permissionManager.GetPermissionOrNull("Inexistente").Returns((Permission)null);

            Should.Throw<AbpValidationException>(() =>
                permissionManager.GetPermissionsFromNamesByValidating(new[] { "Inexistente" }).ToList());
        }
    }
}
