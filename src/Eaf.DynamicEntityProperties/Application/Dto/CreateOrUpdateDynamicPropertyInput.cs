using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eaf.DynamicEntityProperties.Application.Dto
{
    /// <summary>
    /// Input used to create or update a dynamic property and its default values.
    /// </summary>
    public class CreateOrUpdateDynamicPropertyInput : EntityDto
    {
        /// <summary>
        /// Unique name of the dynamic property.
        /// </summary>
        [Required]
        [StringLength(Abp.DynamicEntityProperties.DynamicProperty.MaxPropertyName)]
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
        /// Predefined values for combobox / multiselect input types.
        /// </summary>
        public List<DynamicPropertyValueDto> Values { get; set; }
    }
}
