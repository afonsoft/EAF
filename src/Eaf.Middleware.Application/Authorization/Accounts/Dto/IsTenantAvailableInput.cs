using Abp.MultiTenancy;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa a classe IsTenantAvailableInput.
    /// </summary>
    public class IsTenantAvailableInput
    {
        [Required]
        [MaxLength(AbpTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }
    }
}