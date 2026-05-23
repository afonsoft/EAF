using Eaf.Middleware.Authorization.TwoFactor;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.TwoFactor
{
    public class TwoFactorCacheItemTests
    {
        [Fact]
        public void Dado_CacheName_Quando_Verificar_Entao_DeveSerCorreto()
        {
            TwoFactorCodeCacheItem.CacheName.ShouldBe("AppTwoFactorCodeCache");
        }

        [Fact]
        public void Dado_DefaultSlidingExpireTime_Quando_Verificar_Entao_DeveSerUmaHora()
        {
            TwoFactorCodeCacheItem.DefaultSlidingExpireTime.ShouldBe(TimeSpan.FromHours(1));
        }

        [Fact]
        public void Dado_ConstrutorPadrao_Quando_CriarTwoFactorCodeCacheItem_Entao_CodeDeveSerNull()
        {
            var item = new TwoFactorCodeCacheItem();
            item.Code.ShouldBeNull();
        }

        [Fact]
        public void Dado_ConstrutorComCodigo_Quando_CriarTwoFactorCodeCacheItem_Entao_DeveDefinirCodigo()
        {
            var item = new TwoFactorCodeCacheItem("123456");
            item.Code.ShouldBe("123456");
        }
    }
}
