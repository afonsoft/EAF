using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.MultiTenancy.Dto
{
    /// <summary>
    /// Representa a classe CreateTenantInput.
    /// </summary>
    public class CreateTenantInput
    {
        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string AdminEmailAddress { get; set; }

        [StringLength(AbpUserBase.MaxPasswordLength)]
        [DisableAuditing]
        public string AdminPassword { get; set; }

        /// <summary>
        /// Obtém ou define IsActive.
        /// </summary>
        public bool IsActive { get; set; }

        [Required]
        [StringLength(TenantConsts.MaxNameLength)]
        public string Name { get; set; }

        /// <summary>
        /// Obtém ou define SendActivationEmail.
        /// </summary>
        public bool SendActivationEmail { get; set; }

        /// <summary>
        /// Obtém ou define ShouldChangePasswordOnNextLogin.
        /// </summary>
        public bool ShouldChangePasswordOnNextLogin { get; set; }

        [Required]
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        [RegularExpression(TenantConsts.TenancyNameRegex)]
        public string TenancyName { get; set; }
    }
}