using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Users.Dto
{
    /// <summary>
    /// Representa a classe UserEditDto.
    /// </summary>
    public class UserEditDto : IPassivable
    {
        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Set null to create a new user. Set user's Id to update a user
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Obtém ou define IsActive.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Obtém ou define IsLockoutEnabled.
        /// </summary>
        public virtual bool IsLockoutEnabled { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        /// <summary>
        /// Obtém ou define ShouldChangePasswordOnNextLogin.
        /// </summary>
        public bool ShouldChangePasswordOnNextLogin { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [StringLength(AbpUserBase.MaxPhoneNumberLength)]
        public string PhoneNumber { get; set; }
    }
}