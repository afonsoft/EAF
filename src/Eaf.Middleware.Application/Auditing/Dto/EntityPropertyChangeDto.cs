using Abp.Application.Services.Dto;

namespace Eaf.Middleware.Auditing.Dto
{
    /// <summary>
    /// Representa a classe EntityPropertyChangeDto.
    /// </summary>
    public class EntityPropertyChangeDto : EntityDto<long>
    {
        /// <summary>
        /// Obtém ou define EntityChangeId.
        /// </summary>
        public long EntityChangeId { get; set; }

        /// <summary>
        /// Obtém ou define NewValue.
        /// </summary>
        public string NewValue { get; set; }

        /// <summary>
        /// Obtém ou define OriginalValue.
        /// </summary>
        public string OriginalValue { get; set; }

        /// <summary>
        /// Obtém ou define PropertyName.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Obtém ou define PropertyTypeFullName.
        /// </summary>
        public string PropertyTypeFullName { get; set; }

        public int? TenantId { get; set; }
    }
}