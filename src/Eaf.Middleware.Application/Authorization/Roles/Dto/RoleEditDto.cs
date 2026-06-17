using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Roles.Dto
{
    /// <summary>
    /// DTO (Data Transfer Object) para RoleEdit.
    /// </summary>
    [AutoMap(typeof(Role))]
    public class RoleEditDto
    {
        [Required]
        public string DisplayName { get; set; }

        public int? Id { get; set; }
        /// <summary>
        /// Obtém ou define IsDefault.
        /// </summary>
        public bool IsDefault { get; set; }
    }
}