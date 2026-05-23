using Eaf.Middleware.Dto;
using Abp.Runtime.Validation;

namespace Eaf.Middleware.MultiTenancy.Dto
{
    /// <summary>
    /// Representa a classe GetTenantsInput.
    /// </summary>
    public class GetTenantsInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// Obtém ou define Filter.
        /// </summary>
        public string Filter { get; set; } = "";

        /// <summary>
        /// Normalize.
        /// </summary>
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "TenancyName";
            }
        }
    }
}