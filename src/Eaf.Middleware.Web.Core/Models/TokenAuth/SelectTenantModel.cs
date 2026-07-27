using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Web.Models.TokenAuth
{
    /// <summary>
    /// Modelo para selecionar um tenant e obter o JWT escopado.
    /// </summary>
    public class SelectTenantModel : AvailableTenantsModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int TenantId { get; set; }
    }
}
