using Eaf.Middleware.Sessions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Sessions.Dto
{
    public class UpdateUserSignInTokenOutputTests
    {
        [Fact]
        public void Dado_UpdateUserSignInTokenOutput_Quando_Criado_Entao_PropriedadesDevemSerNulas()
        {
            var output = new UpdateUserSignInTokenOutput();

            output.EncodedTenantId.ShouldBeNull();
            output.EncodedUserId.ShouldBeNull();
            output.SignInToken.ShouldBeNull();
        }

        [Fact]
        public void Dado_UpdateUserSignInTokenOutput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var output = new UpdateUserSignInTokenOutput
            {
                EncodedTenantId = "enc-tenant-1",
                EncodedUserId = "enc-user-42",
                SignInToken = "token-abc123"
            };

            output.EncodedTenantId.ShouldBe("enc-tenant-1");
            output.EncodedUserId.ShouldBe("enc-user-42");
            output.SignInToken.ShouldBe("token-abc123");
        }
    }
}
