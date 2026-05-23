using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    public class GetUserForEditOutputTests
    {
        [Fact]
        public void Dado_GetUserForEditOutput_Quando_Criado_Entao_PropriedadesDevemSerNulas()
        {
            var output = new GetUserForEditOutput();

            output.ProfilePictureId.ShouldBeNull();
            output.Roles.ShouldBeNull();
            output.User.ShouldBeNull();
        }

        [Fact]
        public void Dado_GetUserForEditOutput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var pictureId = Guid.NewGuid();
            var output = new GetUserForEditOutput
            {
                ProfilePictureId = pictureId,
                Roles = new[] { new UserRoleDto { RoleName = "Admin" } },
                User = new UserEditDto { UserName = "admin" }
            };

            output.ProfilePictureId.ShouldBe(pictureId);
            output.Roles.Length.ShouldBe(1);
            output.User.UserName.ShouldBe("admin");
        }
    }
}
