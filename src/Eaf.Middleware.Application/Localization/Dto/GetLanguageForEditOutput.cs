using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace Eaf.Middleware.Localization.Dto
{
    /// <summary>
    /// Representa a classe GetLanguageForEditOutput.
    /// </summary>
    public class GetLanguageForEditOutput
    {
        /// <summary>
        /// GetLanguageForEditOutput.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public GetLanguageForEditOutput()
        {
            LanguageNames = new List<ComboboxItemDto>();
            Flags = new List<ComboboxItemDto>();
        }

        public List<ComboboxItemDto> Flags { get; set; }
        /// <summary>
        /// Obtém ou define Language.
        /// </summary>
        public ApplicationLanguageEditDto Language { get; set; }

        public List<ComboboxItemDto> LanguageNames { get; set; }
    }
}