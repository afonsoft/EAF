using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Editions.Dto
{
    /// <summary>
    /// Entrada para atualização das features de uma Edition.
    /// </summary>
    public class UpdateEditionFeaturesInput
    {
        /// <summary>
        /// Identificador da edição.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        /// <summary>
        /// Valores das features a serem atribuídos à edição.
        /// </summary>
        [Required]
        public List<NameValueDto> FeatureValues { get; set; }
    }
}
