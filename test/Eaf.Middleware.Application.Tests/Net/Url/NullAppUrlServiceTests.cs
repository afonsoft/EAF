using Eaf.Middleware.Url;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Net.Url
{
    public class NullAppUrlServiceTests
    {
        [Fact]
        public void Instance_IsSingleton()
        {
            NullAppUrlService.Instance.ShouldNotBeNull();
            NullAppUrlService.Instance.ShouldBeSameAs(NullAppUrlService.Instance);
        }

        [Fact]
        public void CreateEmailActivationUrlFormat_ThrowsNotImplemented()
        {
            Should.Throw<NotImplementedException>(() => NullAppUrlService.Instance.CreateEmailActivationUrlFormat((int?)1));
            Should.Throw<NotImplementedException>(() => NullAppUrlService.Instance.CreateEmailActivationUrlFormat("tn"));
        }

        [Fact]
        public void CreatePasswordResetUrlFormat_ThrowsNotImplemented()
        {
            Should.Throw<NotImplementedException>(() => NullAppUrlService.Instance.CreatePasswordResetUrlFormat((int?)1));
            Should.Throw<NotImplementedException>(() => NullAppUrlService.Instance.CreatePasswordResetUrlFormat("tn"));
        }
    }
}
