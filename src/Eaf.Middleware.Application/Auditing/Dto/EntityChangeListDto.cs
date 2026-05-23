using Abp.Application.Services.Dto;
using Abp.Events.Bus.Entities;
using System;

namespace Eaf.Middleware.Auditing.Dto
{
    /// <summary>
    /// Representa a classe EntityChangeListDto.
    /// </summary>
    public class EntityChangeListDto : EntityDto<long>
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
        /// ToString.
        /// </summary>
        public string ChangeTypeName => ChangeType.ToString();
        /// <summary>
        /// Obtém ou define EntityChangeSetId.
        /// </summary>
        public long EntityChangeSetId { get; set; }
        /// <summary>
        /// Obtém ou define EntityTypeFullName.
        /// </summary>
        public string EntityTypeFullName { get; set; }
        public long? UserId { get; set; }

        /// <summary>
        /// Obtém ou define UserName.
        /// </summary>
        public string UserName { get; set; }
    }
}