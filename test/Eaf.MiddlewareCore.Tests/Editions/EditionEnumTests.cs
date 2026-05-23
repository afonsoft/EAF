using Eaf.Middleware.Core.Editions;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Editions
{
    public class EditionEnumTests
    {
        [Fact]
        public void Dado_EditionPaymentType_Quando_VerificarValores_Entao_DeveSerCorreto()
        {
            ((int)EditionPaymentType.NewRegistration).ShouldBe(0);
            ((int)EditionPaymentType.BuyNow).ShouldBe(1);
            ((int)EditionPaymentType.Upgrade).ShouldBe(2);
            ((int)EditionPaymentType.Extend).ShouldBe(3);
        }

        [Fact]
        public void Dado_PaymentPeriodType_Quando_VerificarValores_Entao_DeveSerCorreto()
        {
            ((int)PaymentPeriodType.Daily).ShouldBe(1);
            ((int)PaymentPeriodType.Weekly).ShouldBe(7);
            ((int)PaymentPeriodType.Monthly).ShouldBe(30);
            ((int)PaymentPeriodType.Annual).ShouldBe(365);
        }
    }
}
