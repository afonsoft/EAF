using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Liga um usuário host (TenantId == null) a um tenant e ao shadow user criado dentro desse tenant.
    /// </summary>
    [Table("AbpUserTenantMemberships")]
    public class UserTenantMembership : CreationAuditedEntity<long>
    {
        [Required]
        public virtual long UserId { get; set; }

        [Required]
        public virtual int TenantId { get; set; }

        [Required]
        public virtual long TenantUserId { get; set; }

        /// <summary>
        /// Indica se este é o tenant padrão para login automático.
        /// </summary>
        public virtual bool IsDefault { get; set; }
    }
}
