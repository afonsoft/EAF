using Eaf.Middleware.Web.Authentication.Identity;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace Eaf.Middleware.Web.Core.Tests.Authentication.Identity
{
    public class IdentityExtensionsTests
    {
        [Fact]
        public void IdentityExtensions_ShouldBeStaticClass()
        {
            // Arrange & Act
            var type = typeof(IdentityExtensions);

            // Assert
            type.ShouldNotBeNull();
            type.IsAbstract.ShouldBeTrue();
            type.IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void IdentityExtensions_ShouldHaveMethods()
        {
            // Arrange & Act
            var type = typeof(IdentityExtensions);
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            // Assert
            methods.ShouldNotBeNull();
            methods.Length.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void ReplaceClaim_IEnumerable_WhenClaimExists_ShouldReplaceClaim()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("name", "old_name"),
                new Claim("role", "admin")
            };
            var newClaim = new Claim("name", "new_name");

            // Act
            var result = claims.ReplaceClaim(newClaim);

            // Assert
            var resultList = result.ToList();
            resultList.Count.ShouldBe(2);
            resultList.First(c => c.Type == "name").Value.ShouldBe("new_name");
            resultList.First(c => c.Type == "role").Value.ShouldBe("admin");
        }

        [Fact]
        public void ReplaceClaim_IEnumerable_WhenClaimNotExists_ShouldNotChangeClaims()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("role", "admin")
            };
            var newClaim = new Claim("name", "new_name");

            // Act
            var result = claims.ReplaceClaim(newClaim);

            // Assert
            var resultList = result.ToList();
            resultList.Count.ShouldBe(1);
            resultList.First().Type.ShouldBe("role");
        }

        [Fact]
        public void ReplaceClaim_ClaimsIdentity_WhenClaimExists_ShouldReplaceClaim()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("name", "old_name"));
            identity.AddClaim(new Claim("role", "admin"));
            var newClaim = new Claim("name", "new_name");

            // Act
            identity.ReplaceClaim(newClaim);

            // Assert
            var claims = identity.Claims.ToList();
            claims.Count.ShouldBe(2);
            claims.First(c => c.Type == "name").Value.ShouldBe("new_name");
            claims.First(c => c.Type == "role").Value.ShouldBe("admin");
        }

        [Fact]
        public void ReplaceClaim_ClaimsIdentity_WhenClaimNotExists_ShouldAddClaim()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("role", "admin"));
            var newClaim = new Claim("name", "new_name");

            // Act
            identity.ReplaceClaim(newClaim);

            // Assert
            var claims = identity.Claims.ToList();
            claims.Count.ShouldBe(2);
            claims.ShouldContain(c => c.Type == "name" && c.Value == "new_name");
            claims.ShouldContain(c => c.Type == "role" && c.Value == "admin");
        }

        [Fact]
        public void ReplaceClaim_IEnumerable_WithEmptyList_ShouldReturnNewClaim()
        {
            // Arrange
            var claims = new List<Claim>();
            var newClaim = new Claim("name", "test");

            // Act
            var result = claims.ReplaceClaim(newClaim);

            // Assert
            var resultList = result.ToList();
            resultList.Count.ShouldBe(0);
        }

        [Fact]
        public void ReplaceClaim_ClaimsIdentity_WithEmptyIdentity_ShouldAddClaim()
        {
            // Arrange
            var identity = new ClaimsIdentity();
            var newClaim = new Claim("name", "test");

            // Act
            identity.ReplaceClaim(newClaim);

            // Assert
            var claims = identity.Claims.ToList();
            claims.Count.ShouldBe(1);
            claims.First().Type.ShouldBe("name");
            claims.First().Value.ShouldBe("test");
        }
    }
}
