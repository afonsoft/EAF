using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace Eaf.Middleware.Editions.Dto
{
    /// <summary>
    /// Saída para edição das features de uma Edition.
    /// </summary>
    public class GetEditionFeaturesEditOutput
    {
        /// <summary>
        /// Lista de features disponíveis para o escopo de Edition.
        /// </summary>
        public List<FlatFeatureDto> Features { get; set; }

        /// <summary>
        /// Valores atuais das features da edição.
        /// </summary>
        public List<NameValueDto> FeatureValues { get; set; }
    }
}
