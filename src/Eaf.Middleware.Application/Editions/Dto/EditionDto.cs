using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;

namespace Eaf.Middleware.Editions.Dto
{
    /// <summary>
    /// DTO para representar uma Edition.
    /// </summary>
    public class EditionDto : EntityDto<int>
    {
        /// <summary>
        /// Nome exibido da edição.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Identifica se a edição é paga.
        /// </summary>
        public bool IsFree { get; set; }

        /// <summary>
        /// Valor da assinatura mensal.
        /// </summary>
        public decimal? MonthlyPrice { get; set; }

        /// <summary>
        /// Valor da assinatura anual.
        /// </summary>
        public decimal? AnnualPrice { get; set; }

        /// <summary>
        /// Valor da assinatura trimestral.
        /// </summary>
        public decimal? QuarterlyPrice { get; set; }

        /// <summary>
        /// Valor da assinatura semestral.
        /// </summary>
        public decimal? BiannualPrice { get; set; }

        /// <summary>
        /// Valor da assinatura permanente.
        /// </summary>
        public decimal? PermanentPrice { get; set; }

        /// <summary>
        /// Período de pagamento padrão.
        /// </summary>
        public int? DefaultPaymentPeriodType { get; set; }

        /// <summary>
        /// Tempo de trial em dias.
        /// </summary>
        public int? TrialDayCount { get; set; }

        /// <summary>
        /// Dias de carência após expiração.
        /// </summary>
        public int? WaitingDayAfterExpire { get; set; }

        /// <summary>
        /// Identificador da edição de expiração.
        /// </summary>
        public int? ExpiringEditionId { get; set; }
    }
}
