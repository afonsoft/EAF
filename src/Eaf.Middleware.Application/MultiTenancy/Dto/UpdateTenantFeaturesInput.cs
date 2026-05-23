using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.MultiTenancy.Dto
{
    /// <summary>
    /// Representa a classe UpdateTenantFeaturesInput.
    /// </summary>
    public class UpdateTenantFeaturesInput
    {
        [Required]
        public List<NameValueDto> FeatureValues { get; set; }

        [Range(1, int.MaxValue)]
        public int Id { get; set; }
    }
}