using Abp.Application.Editions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eaf.Middleware.Core.Editions
{
    /// <summary>
    /// Extends <see cref="Edition"/> to add subscription features.
    /// </summary>
    public class SubscribableEdition : Edition
    {
        /// <summary>
        /// The edition that will assigned after expire date
        /// </summary>
        public int? ExpiringEditionId { get; set; }

        public decimal? DailyPrice { get; set; }

        public decimal? WeeklyPrice { get; set; }

        public decimal? MonthlyPrice { get; set; }

        public decimal? AnnualPrice { get; set; }

        public int? TrialDayCount { get; set; }

        /// <summary>
        /// The account will be taken an action (termination of tenant account) after the specified days when the subscription is expired.
        /// </summary>
        public int? WaitingDayAfterExpire { get; set; }

        [NotMapped]
        public bool IsFree => !DailyPrice.HasValue && !WeeklyPrice.HasValue && !MonthlyPrice.HasValue && !AnnualPrice.HasValue;

        /// <summary>
        /// HasTrial.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public bool HasTrial()
        {
            if (IsFree)
            {
                return false;
            }

            return TrialDayCount.HasValue && TrialDayCount.Value > 0;
        }

        /// <summary>
        /// GetPaymentAmount.
        /// </summary>
        /// <param name="paymentPeriodType">Parâmetro paymentPeriodType.</param>
        /// <returns>Resultado da operação.</returns>
        public decimal GetPaymentAmount(PaymentPeriodType? paymentPeriodType)
        {
            var amount = GetPaymentAmountOrNull(paymentPeriodType);
            if (!amount.HasValue)
            {
                throw new InvalidOperationException("No price information found for " + DisplayName + " edition!");
            }

            return amount.Value;
        }

        /// <summary>
        /// GetPaymentAmountOrNull.
        /// </summary>
        /// <param name="paymentPeriodType">Parâmetro paymentPeriodType.</param>
        /// <returns>Resultado da operação.</returns>
        public decimal? GetPaymentAmountOrNull(PaymentPeriodType? paymentPeriodType)
        {
            switch (paymentPeriodType)
            {
                case PaymentPeriodType.Daily:
                    return DailyPrice;

                case PaymentPeriodType.Weekly:
                    return WeeklyPrice;

                case PaymentPeriodType.Monthly:
                    return MonthlyPrice;

                case PaymentPeriodType.Annual:
                    return AnnualPrice;

                default:
                    return null;
            }
        }
    }
}