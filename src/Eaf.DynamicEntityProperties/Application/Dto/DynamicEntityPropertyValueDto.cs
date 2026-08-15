using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.DynamicEntityProperties;
using System.ComponentModel.DataAnnotations;

namespace Eaf.DynamicEntityProperties.Application.Dto
{
    /// <summary>
    /// Dynamic entity property value output DTO.
    /// </summary>
    [AutoMapFrom(typeof(DynamicEntityPropertyValue))]
    public class DynamicEntityPropertyValueDto : EntityDto<long>
    {
        /// <summary>
        /// Identifier of the entity instance that owns this value.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string EntityId { get; set; }

        /// <summary>
        /// Identifier of the related dynamic entity property.
        /// </summary>
        public int DynamicEntityPropertyId { get; set; }

        /// <summary>
        /// Value content.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Value { get; set; }

        /// <summary>
        /// Tenant identifier, if any.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// The related dynamic entity property definition.
        /// </summary>
        public DynamicEntityPropertyDto DynamicEntityProperty { get; set; }
    }
}
