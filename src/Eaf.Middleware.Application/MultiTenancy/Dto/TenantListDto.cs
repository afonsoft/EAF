using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using System;

namespace Eaf.Middleware.MultiTenancy.Dto
{
    /// <summary>
    /// Representa a classe TenantListDto.
    /// </summary>
    public class TenantListDto : FullAuditedEntityDto, IPassivable
    {
        /// <summary>
        /// Obtém ou define IsActive.
        /// </summary>
        public bool IsActive { get; set; }

        public DateTime LastModificationDate
        {
            get
            {
                return LastModificationTime == null ? CreationTime : LastModificationTime.Value;
            }
        }

        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Obtém ou define TenancyName.
        /// </summary>
        public string TenancyName { get; set; }
    }
}