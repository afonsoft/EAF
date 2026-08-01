using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Eaf.Middleware.MultiTenancy;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa a classe RegisterInput.
    /// </summary>
    public class RegisterInput
    {
        /// <summary>
        /// Modo de seleção de tenant durante o registro.
        /// </summary>
        [Required]
        public TenantSelectionMode TenantSelectionMode { get; set; }

        /// <summary>
        /// Nome técnico do novo tenant (modo CreateNew).
        /// </summary>
        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        [RegularExpression(TenantConsts.TenancyNameRegex)]
        public string TenancyName { get; set; }

        /// <summary>
        /// Nome de exibição do novo tenant (modo CreateNew).
        /// </summary>
        [StringLength(TenantConsts.MaxNameLength)]
        public string TenantName { get; set; }

        /// <summary>
        /// Id do tenant existente para solicitar ingresso (modo JoinExisting).
        /// </summary>
        public int? ExistingTenantId { get; set; }

        /// <summary>
        /// Mensagem opcional ao solicitar ingresso (modo JoinExisting).
        /// </summary>
        [StringLength(512)]
        public string JoinRequestMessage { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        public string Password { get; set; }
    }
}
