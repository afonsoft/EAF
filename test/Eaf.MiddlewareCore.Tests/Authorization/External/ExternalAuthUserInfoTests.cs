using Eaf.Middleware.Core.Authentication.External;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Authorization.External
{
    public class ExternalAuthUserInfoTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_DefinirPropriedades_Entao_DeveArmazenarCorretamente()
        {
            var jObj = new JObject { ["key"] = "value" };
            var info = new ExternalAuthUserInfo
            {
                EmailAddress = "test@example.com",
                Name = "John",
                Provider = "Google",
                ProviderKey = "12345",
                Surname = "Doe",
                Picture = "https://example.com/pic.jpg",
                AccessCode = "abc123",
                Object = jObj
            };

            info.EmailAddress.ShouldBe("test@example.com");
            info.Name.ShouldBe("John");
            info.Provider.ShouldBe("Google");
            info.ProviderKey.ShouldBe("12345");
            info.Surname.ShouldBe("Doe");
            info.Picture.ShouldBe("https://example.com/pic.jpg");
            info.AccessCode.ShouldBe("abc123");
            info.Object.ShouldBe(jObj);
        }

        [Fact]
        public void Dado_NovaInstancia_Quando_NaoDefinirPropriedades_Entao_DevemSerNull()
        {
            var info = new ExternalAuthUserInfo();
            info.EmailAddress.ShouldBeNull();
            info.Name.ShouldBeNull();
            info.Provider.ShouldBeNull();
            info.ProviderKey.ShouldBeNull();
            info.Surname.ShouldBeNull();
            info.Picture.ShouldBeNull();
            info.AccessCode.ShouldBeNull();
            info.Object.ShouldBeNull();
        }
    }
}
