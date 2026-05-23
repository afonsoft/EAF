using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Accounts.Dto
{
    /// <summary>
    /// Representa a classe SendEmailActivationLinkInput.
    /// </summary>
    public class SendEmailActivationLinkInput
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}