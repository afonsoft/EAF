using Abp.Application.Services.Dto;
using System;

namespace Eaf.Middleware.Localization.Dto
{
    /// <summary>
    /// Representa a classe ApplicationLanguageListDto.
    /// </summary>
    public class ApplicationLanguageListDto : FullAuditedEntityDto
    {
        /// <summary>
        /// Obtém ou define DisplayName.
        /// </summary>
        public virtual string DisplayName { get; set; }
        /// <summary>
        /// Obtém ou define Icon.
        /// </summary>
        public virtual string Icon { get; set; }
        /// <summary>
        /// Obtém ou define IsDisabled.
        /// </summary>
        public bool IsDisabled { get; set; }

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
        public virtual string Name { get; set; }
        public virtual int? TenantId { get; set; }
    }
}