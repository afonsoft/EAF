using Eaf.Middleware.Authorization.Roles.Dto;
using Shouldly;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Roles.Dto
{
    public class CreateOrUpdateRoleInputTests
    {
        [Fact]
        public void Dado_CreateOrUpdateRoleInput_Quando_Criado_Entao_PropriedadesDevemSerNulas()
        {
            var input = new CreateOrUpdateRoleInput();
            input.GrantedPermissionNames.ShouldBeNull();
            input.Role.ShouldBeNull();
        }

        [Fact]
        public void Dado_CreateOrUpdateRoleInput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var permissions = new List<string> { "Pages.Admin", "Pages.Users" };
            var role = new RoleEditDto { DisplayName = "Admin" };

            var input = new CreateOrUpdateRoleInput
            {
                GrantedPermissionNames = permissions,
                Role = role
            };

            input.GrantedPermissionNames.Count.ShouldBe(2);
            input.Role.DisplayName.ShouldBe("Admin");
        }

        [Fact]
        public void Dado_CreateOrUpdateRoleInput_Quando_Verificado_Entao_RoleDeveConterRequiredAttribute()
        {
            var prop = typeof(CreateOrUpdateRoleInput).GetProperty(nameof(CreateOrUpdateRoleInput.Role));
            var attr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            attr.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_CreateOrUpdateRoleInput_Quando_Verificado_Entao_GrantedPermissionsDeveConterRequiredAttribute()
        {
            var prop = typeof(CreateOrUpdateRoleInput).GetProperty(nameof(CreateOrUpdateRoleInput.GrantedPermissionNames));
            var attr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            attr.ShouldNotBeNull();
        }
    }
}
