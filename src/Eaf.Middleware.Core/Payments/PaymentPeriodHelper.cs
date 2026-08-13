using Eaf.Middleware.Core.Editions;
using System;

namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Helper para cálculo de datas e dias de períodos de assinatura.
    /// </summary>
    public static class PaymentPeriodHelper
    {
        /// <summary>
        /// Calcula a data de término a partir da data de início e período.
        /// </summary>
        public static DateTime? GetEndDate(DateTime start, PaymentPeriodType period)
        {
            return period switch
            {
                PaymentPeriodType.Daily => start.AddDays(1),
                PaymentPeriodType.Weekly => start.AddDays(7),
                PaymentPeriodType.Monthly => start.AddMonths(1),
                PaymentPeriodType.Quarterly => start.AddMonths(3),
                PaymentPeriodType.Biannual => start.AddMonths(6),
                PaymentPeriodType.Annual => start.AddYears(1),
                PaymentPeriodType.Permanent => null,
                _ => start,
            };
        }

        /// <summary>
        /// Calcula a duração aproximada em dias de um período.
        /// </summary>
        public static int GetDaysInPeriod(PaymentPeriodType period, DateTime? reference = null)
        {
            var start = reference ?? DateTime.UtcNow;
            var end = GetEndDate(start, period);
            if (!end.HasValue)
            {
                return 365;
            }
            return Math.Max(1, (int)(end.Value - start).TotalDays);
        }
    }
}
