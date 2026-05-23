using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace Eaf.Middleware.Localization.Dto
{
    /// <summary>
    /// Representa a classe GetLanguagesOutput.
    /// </summary>
    public class GetLanguagesOutput : ListResultDto<ApplicationLanguageListDto>
    {
        /// <summary>
        /// GetLanguagesOutput.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public GetLanguagesOutput()
        {
        }

        /// <summary>
        /// GetLanguagesOutput.
        /// </summary>
        /// <param name="items">Parâmetro items.</param>
        /// <param name="defaultLanguageName">Parâmetro defaultLanguageName.</param>
        /// <returns>Resultado da operação.</returns>
        public GetLanguagesOutput(IReadOnlyList<ApplicationLanguageListDto> items, string defaultLanguageName)
            : base(items)
        {
            DefaultLanguageName = defaultLanguageName;
        }

        /// <summary>
        /// Obtém ou define DefaultLanguageName.
        /// </summary>
        public string DefaultLanguageName { get; set; }
    }
}