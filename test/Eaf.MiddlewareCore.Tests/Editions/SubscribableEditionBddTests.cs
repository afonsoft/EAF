using Eaf.Middleware.Core.Editions;
using Shouldly;
using System;
using Xunit;

namespace Eaf.Middleware.Tests.Editions
{
    /// <summary>
    /// Testes BDD para SubscribableEdition seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class SubscribableEditionBddTests
    {
        #region IsFree

        [Fact]
        public void Dado_EdicaoSemPrecos_Quando_VerificarIsFree_Entao_DeveRetornarTrue()
        {
            // Dado
            var edition = new SubscribableEdition();

            // Quando & Então
            edition.IsFree.ShouldBeTrue();
        }

        [Fact]
        public void Dado_EdicaoComPrecoDiario_Quando_VerificarIsFree_Entao_DeveRetornarFalse()
        {
            // Dado
            var edition = new SubscribableEdition { DailyPrice = 9.99m };

            // Quando & Então
            edition.IsFree.ShouldBeFalse();
        }

        [Fact]
        public void Dado_EdicaoComPrecoSemanal_Quando_VerificarIsFree_Entao_DeveRetornarFalse()
        {
            // Dado
            var edition = new SubscribableEdition { WeeklyPrice = 29.99m };

            // Quando & Então
            edition.IsFree.ShouldBeFalse();
        }

        [Fact]
        public void Dado_EdicaoComPrecoMensal_Quando_VerificarIsFree_Entao_DeveRetornarFalse()
        {
            // Dado
            var edition = new SubscribableEdition { MonthlyPrice = 99.99m };

            // Quando & Então
            edition.IsFree.ShouldBeFalse();
        }

        [Fact]
        public void Dado_EdicaoComPrecoAnual_Quando_VerificarIsFree_Entao_DeveRetornarFalse()
        {
            // Dado
            var edition = new SubscribableEdition { AnnualPrice = 999.99m };

            // Quando & Então
            edition.IsFree.ShouldBeFalse();
        }

        #endregion

        #region HasTrial

        [Fact]
        public void Dado_EdicaoGratuitaComTrial_Quando_VerificarHasTrial_Entao_DeveRetornarFalse()
        {
            // Dado
            var edition = new SubscribableEdition { TrialDayCount = 30 };

            // Quando & Então
            edition.HasTrial().ShouldBeFalse();
        }

        [Fact]
        public void Dado_EdicaoPagaComTrial_Quando_VerificarHasTrial_Entao_DeveRetornarTrue()
        {
            // Dado
            var edition = new SubscribableEdition
            {
                MonthlyPrice = 49.99m,
                TrialDayCount = 14
            };

            // Quando & Então
            edition.HasTrial().ShouldBeTrue();
        }

        [Fact]
        public void Dado_EdicaoPagaSemTrial_Quando_VerificarHasTrial_Entao_DeveRetornarFalse()
        {
            // Dado
            var edition = new SubscribableEdition { MonthlyPrice = 49.99m };

            // Quando & Então
            edition.HasTrial().ShouldBeFalse();
        }

        [Fact]
        public void Dado_EdicaoPagaComTrialZero_Quando_VerificarHasTrial_Entao_DeveRetornarFalse()
        {
            // Dado
            var edition = new SubscribableEdition
            {
                MonthlyPrice = 49.99m,
                TrialDayCount = 0
            };

            // Quando & Então
            edition.HasTrial().ShouldBeFalse();
        }

        #endregion

        #region GetPaymentAmount

        [Fact]
        public void Dado_EdicaoComPrecoDiario_Quando_ObterPagamentoDiario_Entao_DeveRetornarValor()
        {
            // Dado
            var edition = new SubscribableEdition { DailyPrice = 5.0m };

            // Quando
            var amount = edition.GetPaymentAmount(PaymentPeriodType.Daily);

            // Então
            amount.ShouldBe(5.0m);
        }

        [Fact]
        public void Dado_EdicaoComPrecoSemanal_Quando_ObterPagamentoSemanal_Entao_DeveRetornarValor()
        {
            // Dado
            var edition = new SubscribableEdition { WeeklyPrice = 25.0m };

            // Quando
            var amount = edition.GetPaymentAmount(PaymentPeriodType.Weekly);

            // Então
            amount.ShouldBe(25.0m);
        }

        [Fact]
        public void Dado_EdicaoComPrecoMensal_Quando_ObterPagamentoMensal_Entao_DeveRetornarValor()
        {
            // Dado
            var edition = new SubscribableEdition { MonthlyPrice = 99.0m };

            // Quando
            var amount = edition.GetPaymentAmount(PaymentPeriodType.Monthly);

            // Então
            amount.ShouldBe(99.0m);
        }

        [Fact]
        public void Dado_EdicaoComPrecoAnual_Quando_ObterPagamentoAnual_Entao_DeveRetornarValor()
        {
            // Dado
            var edition = new SubscribableEdition { AnnualPrice = 999.0m };

            // Quando
            var amount = edition.GetPaymentAmount(PaymentPeriodType.Annual);

            // Então
            amount.ShouldBe(999.0m);
        }

        [Fact]
        public void Dado_EdicaoSemPreco_Quando_ObterPagamento_Entao_DeveLancarExcecao()
        {
            // Dado
            var edition = new SubscribableEdition { DisplayName = "Free" };

            // Quando & Então
            Should.Throw<Exception>(() => edition.GetPaymentAmount(PaymentPeriodType.Monthly));
        }

        #endregion

        #region GetPaymentAmountOrNull

        [Fact]
        public void Dado_EdicaoSemPreco_Quando_ObterPagamentoOuNull_Entao_DeveRetornarNull()
        {
            // Dado
            var edition = new SubscribableEdition();

            // Quando
            var amount = edition.GetPaymentAmountOrNull(PaymentPeriodType.Daily);

            // Então
            amount.ShouldBeNull();
        }

        [Fact]
        public void Dado_PeriodoNull_Quando_ObterPagamentoOuNull_Entao_DeveRetornarNull()
        {
            // Dado
            var edition = new SubscribableEdition { MonthlyPrice = 50m };

            // Quando
            var amount = edition.GetPaymentAmountOrNull(null);

            // Então
            amount.ShouldBeNull();
        }

        #endregion

        #region Propriedades

        [Fact]
        public void Dado_Edicao_Quando_DefinirPropriedades_Entao_DeveArmazenarCorretamente()
        {
            // Dado
            var edition = new SubscribableEdition();

            // Quando
            edition.ExpiringEditionId = 2;
            edition.WaitingDayAfterExpire = 30;
            edition.TrialDayCount = 7;

            // Então
            edition.ExpiringEditionId.ShouldBe(2);
            edition.WaitingDayAfterExpire.ShouldBe(30);
            edition.TrialDayCount.ShouldBe(7);
        }

        #endregion
    }
}
