using Abp.Domain.Entities;
using Abp.MultiTenancy;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eaf.Middleware.Core.Cache
{
    [Table("EafCache")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public class EafCache : IEntity<string>
    {
        [Required]
        [StringLength(449)]
        public string Id { get; set; }

        public byte[] Value { get; set; }
        /// <summary>
        /// Obtém ou define ExpiresAtTime.
        /// </summary>
        public DateTimeOffset ExpiresAtTime { get; set; }
        public long? SlidingExpirationInSeconds { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }

        /// <summary>
        /// IsTransient.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public bool IsTransient()
        {
            return true;
        }
    }
}