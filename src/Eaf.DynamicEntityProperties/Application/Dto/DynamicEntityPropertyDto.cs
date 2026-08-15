using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.DynamicEntityProperties;
using System.ComponentModel.DataAnnotations;

namespace Eaf.DynamicEntityProperties.Application.Dto
{
    /// <summary>
    /// Dynamic entity property output DTO.
    /// </summary>
    [AutoMapFrom(typeof(DynamicEntityProperty))]
    public class DynamicEntityPropertyDto : EntityDto
    {
        /// <summary>
        /// Full CLR type name of the entity to which this property applies.
        /// </summary>
        [Required]
        [StringLength(DynamicEntityProperty.MaxEntityFullName)]
        public string EntityFullName { get; set; }

        /// <summary>
        /// Identifier of the related dynamic property.
        /// </summary>
        public int DynamicPropertyId { get; set; }

        /// <summary>
        /// Tenant identifier, if any.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// The related dynamic property definition.
        /// </summary>
        public DynamicPropertyDto DynamicProperty { get; set; }
    }
}
