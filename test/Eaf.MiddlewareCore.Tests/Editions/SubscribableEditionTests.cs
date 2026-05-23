using Eaf.Middleware.Core.Editions;
using Shouldly;
using System;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Editions
{
    public class SubscribableEditionTests
    {
        [Fact]
        public void Dado_EditionSemPrecos_Quando_VerificarIsFree_Entao_DeveRetornarTrue()
        {
            var edition = new SubscribableEdition();
            edition.IsFree.ShouldBeTrue();
        }

        [Fact]
        public void Dado_EditionComDailyPrice_Quando_VerificarIsFree_Entao_DeveRetornarFalse()
        {
            var edition = new SubscribableEdition { DailyPrice = 10.0m };
            edition.IsFree.ShouldBeFalse();
        }

        [Fact]
        public void Dado_EditionComWeeklyPrice_Quando_VerificarIsFree_Entao_DeveRetornarFalse()
        {
            var edition = new SubscribableEdition { WeeklyPrice = 50.0m };
            edition.IsFree.ShouldBeFalse();
        }

        [Fact]
        public void Dado_EditionComMonthlyPrice_Quando_VerificarIsFree_Entao_DeveRetornarFalse()
        {
            var edition = new SubscribableEdition { MonthlyPrice = 100.0m };
            edition.IsFree.ShouldBeFalse();
        }

        [Fact]
        public void Dado_EditionComAnnualPrice_Quando_VerificarIsFree_Entao_DeveRetornarFalse()
        {
            var edition = new SubscribableEdition { AnnualPrice = 1000.0m };
            edition.IsFree.ShouldBeFalse();
        }

        [Fact]
        public void Dado_EditionFree_Quando_HasTrial_Entao_DeveRetornarFalse()
        {
            var edition = new SubscribableEdition { TrialDayCount = 30 };
            edition.HasTrial().ShouldBeFalse();
        }

        [Fact]
        public void Dado_EditionPagaComTrial_Quando_HasTrial_Entao_DeveRetornarTrue()
        {
            var edition = new SubscribableEdition
            {
                MonthlyPrice = 100.0m,
                TrialDayCount = 30
            };
            edition.HasTrial().ShouldBeTrue();
        }

        [Fact]
        public void Dado_EditionPagaSemTrial_Quando_HasTrial_Entao_DeveRetornarFalse()
        {
            var edition = new SubscribableEdition { MonthlyPrice = 100.0m };
            edition.HasTrial().ShouldBeFalse();
        }

        [Fact]
        public void Dado_EditionPagaComTrialZero_Quando_HasTrial_Entao_DeveRetornarFalse()
        {
            var edition = new SubscribableEdition
            {
                MonthlyPrice = 100.0m,
                TrialDayCount = 0
            };
            edition.HasTrial().ShouldBeFalse();
        }

        [Theory]
        [InlineData(PaymentPeriodType.Daily, 10.0)]
        [InlineData(PaymentPeriodType.Weekly, 50.0)]
        [InlineData(PaymentPeriodType.Monthly, 100.0)]
        [InlineData(PaymentPeriodType.Annual, 1000.0)]
        public void Dado_EditionComPrecos_Quando_GetPaymentAmount_Entao_DeveRetornarPrecoCorreto(
            PaymentPeriodType periodType, double expectedAmount)
        {
            var edition = new SubscribableEdition
            {
                DisplayName = "Test Edition",
                DailyPrice = 10.0m,
                WeeklyPrice = 50.0m,
                MonthlyPrice = 100.0m,
                AnnualPrice = 1000.0m
            };

            edition.GetPaymentAmount(periodType).ShouldBe((decimal)expectedAmount);
        }

        [Fact]
        public void Dado_EditionSemPreco_Quando_GetPaymentAmountNull_Entao_DeveRetornarNull()
        {
            var edition = new SubscribableEdition { DisplayName = "Free" };
            edition.GetPaymentAmountOrNull(PaymentPeriodType.Monthly).ShouldBeNull();
        }

        [Fact]
        public void Dado_EditionSemPreco_Quando_GetPaymentAmount_Entao_DeveLancarException()
        {
            var edition = new SubscribableEdition { DisplayName = "Free" };
            Should.Throw<Exception>(() => edition.GetPaymentAmount(PaymentPeriodType.Monthly));
        }

        [Fact]
        public void Dado_EditionSemPeriodo_Quando_GetPaymentAmountOrNull_Entao_DeveRetornarNull()
        {
            var edition = new SubscribableEdition { DailyPrice = 10.0m };
            edition.GetPaymentAmountOrNull(null).ShouldBeNull();
        }

        [Fact]
        public void Dado_SubscribableEdition_Quando_DefinirPropriedades_Entao_DeveRetornarValoresCorretos()
        {
            var edition = new SubscribableEdition
            {
                ExpiringEditionId = 5,
                WaitingDayAfterExpire = 30,
                TrialDayCount = 14
            };

            edition.ExpiringEditionId.ShouldBe(5);
            edition.WaitingDayAfterExpire.ShouldBe(30);
            edition.TrialDayCount.ShouldBe(14);
        }

        [Fact]
        public void Dado_PaymentPeriodTypeEnum_Quando_VerificarValores_Entao_DeveSerCorreto()
        {
            ((int)PaymentPeriodType.Daily).ShouldBe(1);
            ((int)PaymentPeriodType.Weekly).ShouldBe(7);
            ((int)PaymentPeriodType.Monthly).ShouldBe(30);
            ((int)PaymentPeriodType.Annual).ShouldBe(365);
        }

        [Fact]
        public void Dado_EditionPaymentTypeEnum_Quando_VerificarValores_Entao_DeveSerCorreto()
        {
            ((int)EditionPaymentType.NewRegistration).ShouldBe(0);
            ((int)EditionPaymentType.BuyNow).ShouldBe(1);
            ((int)EditionPaymentType.Upgrade).ShouldBe(2);
            ((int)EditionPaymentType.Extend).ShouldBe(3);
        }
    }
}
