using Abp.Application.Services.Dto;
using Abp.MultiTenancy;
using System;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.MultiTenancy.Dto
{
    /// <summary>
    /// Representa a classe TenantEditDto.
    /// </summary>
    public class TenantEditDto : EntityDto
    {
        /// <summary>
        /// Obtém ou define IsActive.
        /// </summary>
        public bool IsActive { get; set; }

        [Required]
        [StringLength(TenantConsts.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }

        /// <summary>
        /// Identificador da edição associada ao tenant.
        /// </summary>
        public int? EditionId { get; set; }

        /// <summary>
        /// Data de término da assinatura do tenant (UTC).
        /// </summary>
        public DateTime? SubscriptionEndDateUtc { get; set; }
    }
}