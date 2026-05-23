using Eaf.Middleware.Sessions.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Sessions.Dto
{
    public class ApplicationInfoDtoTests
    {
        [Fact]
        public void Dado_ApplicationInfoDto_Quando_Criado_Entao_PropriedadesDevemSerPadrao()
        {
            var dto = new ApplicationInfoDto();

            dto.Currency.ShouldBeNull();
            dto.CurrencySign.ShouldBeNull();
            dto.Features.ShouldBeNull();
            dto.ReleaseDate.ShouldBe(default(DateTime));
            dto.TwoFactorCodeExpireSeconds.ShouldBe(0.0);
            dto.Version.ShouldBeNull();
        }

        [Fact]
        public void Dado_ApplicationInfoDto_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var now = DateTime.UtcNow;
            var dto = new ApplicationInfoDto
            {
                Currency = "BRL",
                CurrencySign = "R$",
                Features = new Dictionary<string, bool> { { "Chat", true }, { "SignalR", false } },
                ReleaseDate = now,
                TwoFactorCodeExpireSeconds = 300.0,
                Version = "10.0.0"
            };

            dto.Currency.ShouldBe("BRL");
            dto.CurrencySign.ShouldBe("R$");
            dto.Features.Count.ShouldBe(2);
            dto.Features["Chat"].ShouldBeTrue();
            dto.ReleaseDate.ShouldBe(now);
            dto.TwoFactorCodeExpireSeconds.ShouldBe(300.0);
            dto.Version.ShouldBe("10.0.0");
        }
    }
}
