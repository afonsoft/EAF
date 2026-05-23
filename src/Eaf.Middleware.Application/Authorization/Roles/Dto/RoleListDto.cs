using Abp.Application.Services.Dto;
using System;

namespace Eaf.Middleware.Authorization.Roles.Dto
{
    /// <summary>
    /// Representa a classe RoleListDto.
    /// </summary>
    public class RoleListDto : FullAuditedEntityDto
    {
        /// <summary>
        /// Obtém ou define DisplayName.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Obtém ou define IsDefault.
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// Obtém ou define IsStatic.
        /// </summary>
        public bool IsStatic { get; set; }

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
    }
}