using Eaf.Middleware.Authorization.TwoFactor;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.TwoFactor
{
    public class TwoFactorCodeCacheItemTests
    {
        [Fact]
        public void Dado_TwoFactorCodeCacheItem_Quando_CriadoSemParametros_Entao_CodeDeveSerNulo()
        {
            var item = new TwoFactorCodeCacheItem();
            item.Code.ShouldBeNull();
        }

        [Fact]
        public void Dado_TwoFactorCodeCacheItem_Quando_CriadoComCode_Entao_CodeDeveSerAtribuido()
        {
            var item = new TwoFactorCodeCacheItem("123456");
            item.Code.ShouldBe("123456");
        }

        [Fact]
        public void Dado_TwoFactorCodeCacheItem_Quando_VerificarCacheName_Entao_DeveSerAppTwoFactorCodeCache()
        {
            TwoFactorCodeCacheItem.CacheName.ShouldBe("AppTwoFactorCodeCache");
        }

        [Fact]
        public void Dado_TwoFactorCodeCacheItem_Quando_VerificarDefaultSlidingExpireTime_Entao_DeveSerUmaHora()
        {
            TwoFactorCodeCacheItem.DefaultSlidingExpireTime.ShouldBe(TimeSpan.FromHours(1));
        }

        [Fact]
        public void Dado_TwoFactorCodeCacheItem_Quando_Verificado_Entao_DeveConterSerializableAttribute()
        {
            var attr = typeof(TwoFactorCodeCacheItem).GetCustomAttributes(typeof(SerializableAttribute), false);
            attr.ShouldNotBeEmpty();
        }
    }
}
