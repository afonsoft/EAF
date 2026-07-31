using Abp.Application.Services.Dto;
using System;

namespace Eaf.Middleware.UserDelegations.Dto
{
    /// <summary>
    /// DTO para representar uma delegação de usuário.
    /// </summary>
    public class UserDelegationDto : EntityDto<long>
    {
        /// <summary>
        /// Identificador do tenant.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Identificador do usuário que delegou.
        /// </summary>
        public long SourceUserId { get; set; }

        /// <summary>
        /// Identificador do usuário destino.
        /// </summary>
        public long TargetUserId { get; set; }

        /// <summary>
        /// Nome de usuário do usuário que delegou.
        /// </summary>
        public string SourceUserName { get; set; }

        /// <summary>
        /// Nome de usuário do usuário destino.
        /// </summary>
        public string TargetUserName { get; set; }

        /// <summary>
        /// Data/hora inicial.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Data/hora final.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indica se a delegação está ativa.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
