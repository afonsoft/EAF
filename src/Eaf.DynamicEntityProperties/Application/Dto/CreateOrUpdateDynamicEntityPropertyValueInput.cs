using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Eaf.DynamicEntityProperties.Application.Dto
{
    /// <summary>
    /// Input used to create or update a dynamic entity property value.
    /// </summary>
    public class CreateOrUpdateDynamicEntityPropertyValueInput : EntityDto<long>
    {
        /// <summary>
        /// Identifier of the entity instance that owns this value.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string EntityId { get; set; }

        /// <summary>
        /// Identifier of the related dynamic entity property.
        /// </summary>
        [Required]
        public int DynamicEntityPropertyId { get; set; }

        /// <summary>
        /// Value content.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Value { get; set; }
    }
}
