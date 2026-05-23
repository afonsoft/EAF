using Abp.Extensions;
using Abp.Runtime.Validation;
using Eaf.Middleware.Dto;
using System;

namespace Eaf.Middleware.Auditing.Dto
{
    /// <summary>
    /// Representa a classe GetAuditLogsInput.
    /// </summary>
    public class GetAuditLogsInput : PagedAndSortedInputDto, IShouldNormalize
    {
        /// <summary>
        /// Obtém ou define BrowserInfo.
        /// </summary>
        public string BrowserInfo { get; set; }
        /// <summary>
        /// Obtém ou define EndDate.
        /// </summary>
        public DateTime EndDate { get; set; }
        public bool? HasException { get; set; }
        public int? MaxExecutionDuration { get; set; }
        /// <summary>
        /// Obtém ou define MethodName.
        /// </summary>
        public string MethodName { get; set; }
        public int? MinExecutionDuration { get; set; }
        /// <summary>
        /// Obtém ou define ServiceName.
        /// </summary>
        public string ServiceName { get; set; }
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
                Sorting = "ExecutionTime DESC";
            }

            if (Sorting.IndexOf("UserName", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Sorting = "User." + Sorting;
            }
            else
            {
                Sorting = "AuditLog." + Sorting;
            }
        }
    }
}