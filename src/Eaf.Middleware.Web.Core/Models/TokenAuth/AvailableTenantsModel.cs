using Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Web.Models.TokenAuth
{
    /// <summary>
    /// Modelo para listar os tenants disponíveis para um usuário host.
    /// </summary>
    public class AvailableTenantsModel
    {
        [Required]
        [MaxLength(AbpUserBase.MaxEmailAddressLength)]
        public string UserNameOrEmailAddress { get; set; }

        [Required]
        [MaxLength(AbpUserBase.MaxPlainPasswordLength)]
        public string Password { get; set; }
    }
}
