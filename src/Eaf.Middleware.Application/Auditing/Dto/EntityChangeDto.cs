using Abp.Application.Services.Dto;
using Abp.Events.Bus.Entities;
using System;

namespace Eaf.Middleware.Auditing.Dto
{
    /// <summary>
    /// Representa a classe EntityChangeDto.
    /// </summary>
    public class EntityChangeDto : EntityDto<long>
    {
        /// <summary>
        /// Obtém ou define ChangeTime.
        /// </summary>
        public DateTime ChangeTime { get; set; }

        /// <summary>
        /// Obtém ou define ChangeType.
        /// </summary>
        public EntityChangeType ChangeType { get; set; }

        /// <summary>
        /// Obtém ou define EntityChangeSetId.
        /// </summary>
        public long EntityChangeSetId { get; set; }

        /// <summary>
        /// Obtém ou define EntityEntry.
        /// </summary>
        public object EntityEntry { get; set; }
        /// <summary>
        /// Obtém ou define EntityId.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// Obtém ou define EntityTypeFullName.
        /// </summary>
        public string EntityTypeFullName { get; set; }

        public int? TenantId { get; set; }
    }
}