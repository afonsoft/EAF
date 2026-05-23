using Eaf.Middleware.Authorization.TwoFactor;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.TwoFactor
{
    public class TwoFactorCodeCacheTests
    {
        [Fact]
        public void DefaultCtor_AllowsSettingCode()
        {
            var item = new TwoFactorCodeCacheItem { Code = "123" };
            item.Code.ShouldBe("123");
        }

        [Fact]
        public void ParameterizedCtor_SetsCode()
        {
            var item = new TwoFactorCodeCacheItem("abc");
            item.Code.ShouldBe("abc");
        }

        [Fact]
        public void CacheName_ShouldBeConstant()
        {
            TwoFactorCodeCacheItem.CacheName.ShouldBe("AppTwoFactorCodeCache");
        }

        [Fact]
        public void DefaultSlidingExpireTime_ShouldBeOneHour()
        {
            TwoFactorCodeCacheItem.DefaultSlidingExpireTime.ShouldBe(TimeSpan.FromHours(1));
        }
    }
}
