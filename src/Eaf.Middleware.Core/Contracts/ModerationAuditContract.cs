using System;

namespace Eaf.Middleware.Contracts
{
    /// <summary>
    /// Contract for moderation audit entries shared across services.
    /// </summary>
    public class ModerationAuditContract
    {
        /// <summary>
        /// Unique identifier of the audit entry.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Type of moderation action (e.g., approve, reject, ban).
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Identifier of the moderator performing the action.
        /// </summary>
        public long? ModeratorUserId { get; set; }

        /// <summary>
        /// Tenant of the moderator.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Identifier of the moderated resource.
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// Type of the moderated resource (e.g., user_content, report).
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// Reason or note provided for the action.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// UTC timestamp when the action was performed.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
