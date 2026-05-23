using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    public class UpdateUserPermissionsInputTests
    {
        [Fact]
        public void Dado_UpdateUserPermissionsInput_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var input = new UpdateUserPermissionsInput();

            input.GrantedPermissionNames.ShouldBeNull();
            input.Id.ShouldBe(0L);
        }

        [Fact]
        public void Dado_UpdateUserPermissionsInput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var input = new UpdateUserPermissionsInput
            {
                GrantedPermissionNames = new List<string> { "Pages.Admin", "Pages.Users" },
                Id = 42L
            };

            input.GrantedPermissionNames.Count.ShouldBe(2);
            input.Id.ShouldBe(42L);
        }

        [Fact]
        public void Dado_UpdateUserPermissionsInput_Quando_Verificado_Entao_GrantedPermissionsDeveConterRequiredAttribute()
        {
            var prop = typeof(UpdateUserPermissionsInput).GetProperty(nameof(UpdateUserPermissionsInput.GrantedPermissionNames));
            prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault().ShouldNotBeNull();
        }

        [Fact]
        public void Dado_UpdateUserPermissionsInput_Quando_Verificado_Entao_IdDeveConterRangeAttribute()
        {
            var prop = typeof(UpdateUserPermissionsInput).GetProperty(nameof(UpdateUserPermissionsInput.Id));
            var attr = prop!.GetCustomAttributes(typeof(RangeAttribute), false).FirstOrDefault() as RangeAttribute;
            attr.ShouldNotBeNull();
            attr!.Minimum.ShouldBe(1);
        }
    }
}
