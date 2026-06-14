using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    public class ActiveDirectoryLdapDtoBddTests
    {
        [Fact]
        public void Dado_CreateActiveDirectoryUserInput_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var input = new CreateActiveDirectoryUserInput
            {
                UserNames = new[] { "john.doe", "jane.doe" },
                AssignedRoleNames = new[] { "Admin", "User" },
                IsActive = true
            };

            input.UserNames.Length.ShouldBe(2);
            input.UserNames[0].ShouldBe("john.doe");
            input.AssignedRoleNames.Length.ShouldBe(2);
            input.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void Dado_CreateLdapUserInput_Quando_DefinirPropriedades_Entao_DevePersistir()
        {
            var input = new CreateLdapUserInput
            {
                UserNames = new[] { "user1" },
                AssignedRoleNames = new[] { "User" },
                IsActive = false
            };

            input.UserNames.Length.ShouldBe(1);
            input.AssignedRoleNames.Length.ShouldBe(1);
            input.IsActive.ShouldBeFalse();
        }
    }
}
