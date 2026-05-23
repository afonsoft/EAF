using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace Eaf.Middleware.Localization.Dto
{
    /// <summary>
    /// Representa a classe GetLanguagesInput.
    /// </summary>
    public class GetLanguagesInput : ISortedResultRequest, IShouldNormalize
    {
        /// <summary>
        /// Obtém ou define Filter.
        /// </summary>
        public string Filter { get; set; } = "";

        /// <summary>
        /// Obtém ou define Sorting.
        /// </summary>
        public string Sorting { get; set; }

        /// <summary>
        /// Normalize.
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Name";
            }
        }
    }
}