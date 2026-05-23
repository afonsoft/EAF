using Eaf.Middleware;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests
{
    public class MiddlewareCoreConstsTests
    {
        [Fact]
        public void DefaultPassPhrase_ShouldNotBeEmpty()
        {
            // Act
            var defaultPassPhrase = MiddlewareCoreConsts.DefaultPassPhrase;

            // Assert
            defaultPassPhrase.ShouldNotBeNull();
            defaultPassPhrase.ShouldNotBeEmpty();
        }

        [Fact]
        public void DefaultPassPhrase_ShouldBeGsKxGZ012HLL3MI5()
        {
            // Act
            var defaultPassPhrase = MiddlewareCoreConsts.DefaultPassPhrase;

            // Assert
            defaultPassPhrase.ShouldBe("gsKxGZ012HLL3MI5");
        }

        [Fact]
        public void SecurityStampKey_ShouldNotBeEmpty()
        {
            // Act
            var securityStampKey = MiddlewareCoreConsts.SecurityStampKey;

            // Assert
            securityStampKey.ShouldNotBeNull();
            securityStampKey.ShouldNotBeEmpty();
        }

        [Fact]
        public void SecurityStampKey_ShouldBeAspNetIdentitySecurityStamp()
        {
            // Act
            var securityStampKey = MiddlewareCoreConsts.SecurityStampKey;

            // Assert
            securityStampKey.ShouldBe("AspNet.Identity.SecurityStamp");
        }

        [Fact]
        public void TokenValidityKey_ShouldNotBeEmpty()
        {
            // Act
            var tokenValidityKey = MiddlewareCoreConsts.TokenValidityKey;

            // Assert
            tokenValidityKey.ShouldNotBeNull();
            tokenValidityKey.ShouldNotBeEmpty();
        }

        [Fact]
        public void TokenValidityKey_ShouldBeTokenValidityKey()
        {
            // Act
            var tokenValidityKey = MiddlewareCoreConsts.TokenValidityKey;

            // Assert
            tokenValidityKey.ShouldBe("token_validity_key");
        }

        [Fact]
        public void TokenValidityValue_ShouldNotBeEmpty()
        {
            // Act
            var tokenValidityValue = MiddlewareCoreConsts.TokenValidityValue;

            // Assert
            tokenValidityValue.ShouldNotBeNull();
            tokenValidityValue.ShouldNotBeEmpty();
        }

        [Fact]
        public void TokenValidityValue_ShouldBeTokenValidityValue()
        {
            // Act
            var tokenValidityValue = MiddlewareCoreConsts.TokenValidityValue;

            // Assert
            tokenValidityValue.ShouldBe("token_validity_value");
        }

        [Fact]
        public void UserIdentifier_ShouldNotBeEmpty()
        {
            // Act
            var userIdentifier = MiddlewareCoreConsts.UserIdentifier;

            // Assert
            userIdentifier.ShouldNotBeNull();
            userIdentifier.ShouldNotBeEmpty();
        }

        [Fact]
        public void UserIdentifier_ShouldBeUserIdentifier()
        {
            // Act
            var userIdentifier = MiddlewareCoreConsts.UserIdentifier;

            // Assert
            userIdentifier.ShouldBe("user_identifier");
        }

        [Fact]
        public void MiddlewareCoreConsts_ShouldBeStaticClass()
        {
            // Arrange & Act
            var type = typeof(MiddlewareCoreConsts);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }
    }
}
