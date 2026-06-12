using Eaf.Middleware.Authorization.TwoFactor;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization.TwoFactor
{
    /// <summary>
    /// Testes BDD para TwoFactorCodeCacheItem seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class TwoFactorCodeCacheItemBddTests
    {
        [Fact]
        public void Dado_ConstrutorPadrao_Quando_Criar_Entao_CodeDeveSerNull()
        {
            // Dado & Quando
            var item = new TwoFactorCodeCacheItem();

            // Então
            item.Code.ShouldBeNull();
        }

        [Fact]
        public void Dado_ConstrutorComCode_Quando_Criar_Entao_DeveArmazenarCode()
        {
            // Dado & Quando
            var item = new TwoFactorCodeCacheItem("123456");

            // Então
            item.Code.ShouldBe("123456");
        }

        [Fact]
        public void Dado_CacheName_Quando_Verificar_Entao_DeveSerAppTwoFactorCodeCache()
        {
            TwoFactorCodeCacheItem.CacheName.ShouldBe("AppTwoFactorCodeCache");
        }

        [Fact]
        public void Dado_DefaultSlidingExpireTime_Quando_Verificar_Entao_DeveSerUmaHora()
        {
            TwoFactorCodeCacheItem.DefaultSlidingExpireTime.ShouldBe(TimeSpan.FromHours(1));
        }
    }
}
