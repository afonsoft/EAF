using Abp.Application.Services.Dto;
using Eaf.Middleware.Editions.Dto;
using System.Collections.Generic;

namespace Eaf.Middleware.MultiTenancy.Dto
{
    /// <summary>
    /// Representa a classe GetTenantFeaturesEditOutput.
    /// </summary>
    public class GetTenantFeaturesEditOutput
    {
        public List<FlatFeatureDto> Features { get; set; }
        public List<NameValueDto> FeatureValues { get; set; }
    }
}