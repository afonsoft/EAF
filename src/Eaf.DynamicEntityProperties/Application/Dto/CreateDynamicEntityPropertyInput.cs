using System.ComponentModel.DataAnnotations;

namespace Eaf.DynamicEntityProperties.Application.Dto
{
    /// <summary>
    /// Input used to bind a dynamic property to an entity type.
    /// </summary>
    public class CreateDynamicEntityPropertyInput
    {
        /// <summary>
        /// Full CLR type name of the target entity.
        /// </summary>
        [Required]
        [StringLength(Abp.DynamicEntityProperties.DynamicEntityProperty.MaxEntityFullName)]
        public string EntityFullName { get; set; }

        /// <summary>
        /// Identifier of the dynamic property to bind.
        /// </summary>
        [Required]
        public int DynamicPropertyId { get; set; }
    }
}
