using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.DynamicEntityProperties;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eaf.DynamicEntityProperties.Application.Dto
{
    /// <summary>
    /// Dynamic property output DTO.
    /// </summary>
    [AutoMapFrom(typeof(DynamicProperty))]
    public class DynamicPropertyDto : EntityDto
    {
        /// <summary>
        /// Unique name of the dynamic property.
        /// </summary>
        [Required]
        [StringLength(DynamicProperty.MaxPropertyName)]
        public string PropertyName { get; set; }

        /// <summary>
        /// Display name shown in the UI.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Input type name (e.g., SingleLineStringInputType).
        /// </summary>
        public string InputType { get; set; }

        /// <summary>
        /// Optional permission required to view or edit this property.
        /// </summary>
        public string Permission { get; set; }

        /// <summary>
        /// Tenant identifier, if any.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Predefined values for combobox / multiselect input types.
        /// </summary>
        public List<DynamicPropertyValueDto> Values { get; set; }
    }
}
