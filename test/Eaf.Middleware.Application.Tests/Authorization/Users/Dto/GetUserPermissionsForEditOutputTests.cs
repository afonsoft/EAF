using Eaf.Middleware.Authorization.Permissions.Dto;
using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    public class GetUserPermissionsForEditOutputTests
    {
        [Fact]
        public void Dado_GetUserPermissionsForEditOutput_Quando_Criado_Entao_PropriedadesDevemSerNulas()
        {
            var output = new GetUserPermissionsForEditOutput();

            output.GrantedPermissionNames.ShouldBeNull();
            output.Permissions.ShouldBeNull();
        }

        [Fact]
        public void Dado_GetUserPermissionsForEditOutput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var output = new GetUserPermissionsForEditOutput
            {
                GrantedPermissionNames = new List<string> { "Pages.Admin" },
                Permissions = new List<FlatPermissionDto>
                {
                    new FlatPermissionDto { Name = "Pages.Admin" }
                }
            };

            output.GrantedPermissionNames.Count.ShouldBe(1);
            output.Permissions.Count.ShouldBe(1);
        }
    }
}
