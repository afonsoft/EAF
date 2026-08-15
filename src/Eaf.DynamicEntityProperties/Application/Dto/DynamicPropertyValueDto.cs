using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.DynamicEntityProperties;
using System.ComponentModel.DataAnnotations;

namespace Eaf.DynamicEntityProperties.Application.Dto
{
    /// <summary>
    /// Dynamic property value output DTO.
    /// </summary>
    [AutoMapFrom(typeof(DynamicPropertyValue))]
    public class DynamicPropertyValueDto : EntityDto<long>
    {
        /// <summary>
        /// The dynamic property identifier this value belongs to.
        /// </summary>
        public int DynamicPropertyId { get; set; }

        /// <summary>
        /// Value content.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Value { get; set; }

        /// <summary>
        /// Tenant identifier, if any.
        /// </summary>
        public int? TenantId { get; set; }
    }
}
