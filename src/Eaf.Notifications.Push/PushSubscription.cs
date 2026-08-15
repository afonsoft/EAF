using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;

namespace Eaf.Notifications.Push
{
    /// <summary>
    /// Stores a browser/device push subscription for a user.
    /// </summary>
    [Table("EafPushSubscriptions")]
    public class PushSubscription : Entity<long>, IHasCreationTime, IMayHaveTenant
    {
        /// <summary>
        /// User identifier.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Tenant identifier. Null for host users.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Push endpoint URL.
        /// </summary>
        [Required]
        [StringLength(2048)]
        public string Endpoint { get; set; }

        /// <summary>
        /// P-256 DH public key (Base64URL).
        /// </summary>
        [Required]
        [StringLength(256)]
        public string P256dh { get; set; }

        /// <summary>
        /// Authentication secret (Base64URL).
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Auth { get; set; }

        /// <summary>
        /// Creation timestamp.
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
