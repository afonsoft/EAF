using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eaf.Middleware.UserDelegations
{
    /// <summary>
    /// Representa uma delegação de usuário por período limitado.
    /// </summary>
    [Table("EafUserDelegations")]
    public class UserDelegation : FullAuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxDescriptionLength = 1024;

        /// <summary>
        /// Identificador do tenant.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Identificador do usuário que delegou a conta.
        /// </summary>
        public long SourceUserId { get; set; }

        /// <summary>
        /// Identificador do usuário destino da delegação.
        /// </summary>
        public long TargetUserId { get; set; }

        /// <summary>
        /// Data/hora inicial da delegação.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Data/hora final da delegação.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Descrição opcional da delegação.
        /// </summary>
        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }

        /// <summary>
        /// Verifica se a delegação está ativa para o momento informado.
        /// </summary>
        /// <param name="now">Momento a ser verificado.</param>
        /// <returns>True se estiver ativa.</returns>
        public bool IsActive(DateTime now)
        {
            return !IsDeleted && StartTime <= now && EndTime >= now;
        }
    }
}
