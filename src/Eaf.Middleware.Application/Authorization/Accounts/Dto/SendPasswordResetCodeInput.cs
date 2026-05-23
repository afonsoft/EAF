using Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa a classe SendPasswordResetCodeInput.
    /// </summary>
    public class SendPasswordResetCodeInput
    {
        [Required]
        [MaxLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
    }
}