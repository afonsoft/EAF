using Abp.Extensions;
using Abp.Runtime.Validation;
using Eaf.Middleware.Dto;
using System;

namespace Eaf.Middleware.Auditing.Dto
{
    /// <summary>
    /// Representa a classe GetEntityChangeInput.
    /// </summary>
    public class GetEntityChangeInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// Obtém ou define EndDate.
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// Obtém ou define EntityTypeFullName.
        /// </summary>
        public string EntityTypeFullName { get; set; }
        /// <summary>
        /// Obtém ou define StartDate.
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Obtém ou define UserName.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Normalize.
        /// </summary>
        public void Normalize()
        {
            if (Sorting.IsNullOrWhiteSpace())
            {
                Sorting = "ChangeTime DESC";
            }

            if (Sorting.IndexOf("UserName", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Sorting = "User." + Sorting;
            }
            else
            {
                Sorting = "EntityChange." + Sorting;
            }
        }
    }

    /// <summary>
    /// Representa a classe GetEntityTypeChangeInput.
    /// </summary>
    public class GetEntityTypeChangeInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// Obtém ou define EntityId.
        /// </summary>
        public string EntityId { get; set; }
        /// <summary>
        /// Obtém ou define EntityTypeFullName.
        /// </summary>
        public string EntityTypeFullName { get; set; }

        /// <summary>
        /// Normalize.
        /// </summary>
        public void Normalize()
        {
            if (Sorting.IsNullOrWhiteSpace())
            {
                Sorting = "ChangeTime DESC";
            }

            if (Sorting.IndexOf("UserName", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Sorting = "User." + Sorting;
            }
            else
            {
                Sorting = "EntityChange." + Sorting;
            }
        }
    }
}