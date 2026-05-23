using Eaf.Middleware.Authorization.Permissions.Dto;
using Eaf.Middleware.Authorization.Roles.Dto;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Roles.Dto
{
    public class GetRoleForEditOutputTests
    {
        [Fact]
        public void Dado_GetRoleForEditOutput_Quando_Criado_Entao_PropriedadesDevemSerNulas()
        {
            var output = new GetRoleForEditOutput();

            output.GrantedPermissionNames.ShouldBeNull();
            output.Permissions.ShouldBeNull();
            output.Role.ShouldBeNull();
        }

        [Fact]
        public void Dado_GetRoleForEditOutput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var output = new GetRoleForEditOutput
            {
                GrantedPermissionNames = new List<string> { "Pages.Admin" },
                Permissions = new List<FlatPermissionDto>
                {
                    new FlatPermissionDto { Name = "Pages.Admin", DisplayName = "Admin" }
                },
                Role = new RoleEditDto { DisplayName = "Admin", Id = 1 }
            };

            output.GrantedPermissionNames.Count.ShouldBe(1);
            output.Permissions.Count.ShouldBe(1);
            output.Role.DisplayName.ShouldBe("Admin");
        }
    }
}
