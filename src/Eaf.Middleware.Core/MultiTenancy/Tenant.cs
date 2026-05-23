using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using Eaf.Middleware.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Representa a classe Tenant.
    /// </summary>
    public partial class Tenant : AbpTenant<User>
    {
        public virtual ICollection<TenantAddress> Addresses { get; set; }
        /// <summary>
        /// Tenant.
        /// </summary>
        /// <param name="tenancyName">Parâmetro tenancyName.</param>
        /// <param name="name">Parâmetro name.</param>
        /// <returns>Resultado da operação.</returns>
        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }

        protected Tenant()
        {
        }
    }
    /// <summary>
    /// Base class for tenant address with optimized performance.
    /// </summary>
    [Table("AbpTenantAddress")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public partial class TenantAddress : FullAuditedEntity<int>, IMayHaveTenant, IExtendableObject
    {
        /// <summary>
        /// Gets or sets the zip code. Optimized with fixed length.
        /// </summary>
        [Required]
        [StringLength(10)]
        public string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets the street address. Optimized with fixed length.
        /// </summary>
        [Required]
        [StringLength(512)]
        public string Street { get; set; }

        /// <summary>
        /// Gets or sets the neighborhood. Optimized with fixed length.
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Neighborhood { get; set; }

        /// <summary>
        /// Gets or sets the city. Optimized with fixed length.
        /// </summary>
        [Required]
        [StringLength(256)]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the state. Optimized with fixed length.
        /// </summary>
        [Required]
        [StringLength(4)]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the complement. Optional field with optimized length.
        /// </summary>
        [StringLength(256)]
        public string Complement { get; set; }

        /// <summary>
        /// Gets or sets the observation. Optional field with optimized length.
        /// </summary>
        [StringLength(512)]
        public string Observation { get; set; }

        /// <summary>
        /// Gets or sets the email. Optimized with proper validation.
        /// </summary>
        [StringLength(512)]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the document. Optimized with fixed length.
        /// </summary>
        [StringLength(256)]
        public string Document { get; set; }

        /// <summary>
        /// Gets or sets the extension data. Optimized for JSON serialization.
        /// </summary>
        [StringLength(2000)]
        public string ExtensionData { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier for multi-tenancy support.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Gets or sets whether the address is active. Default is true.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Navigation property to the tenant. Configured for lazy loading.
        /// </summary>
        public virtual Tenant Tenant { get; set; }

        /// <summary>
        /// Optimized method to validate the address data.
        /// </summary>
        /// <returns>True if valid, false otherwise.</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ZipCode) &&
                   !string.IsNullOrEmpty(Street) &&
                   !string.IsNullOrEmpty(Neighborhood) &&
                   !string.IsNullOrEmpty(City) &&
                   !string.IsNullOrEmpty(State);
        }

        /// <summary>
        /// Optimized method to get full address string.
        /// </summary>
        /// <returns>Formatted full address.</returns>
        public string GetFullAddress()
        {
            var addressBuilder = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(Street))
                addressBuilder.Append(Street);

            if (!string.IsNullOrEmpty(Neighborhood))
            {
                if (addressBuilder.Length > 0)
                    addressBuilder.Append(", ");
                addressBuilder.Append(Neighborhood);
            }

            if (!string.IsNullOrEmpty(City))
            {
                if (addressBuilder.Length > 0)
                    addressBuilder.Append(", ");
                addressBuilder.Append(City);
            }

            if (!string.IsNullOrEmpty(State))
            {
                if (addressBuilder.Length > 0)
                    addressBuilder.Append(" - ");
                addressBuilder.Append(State);
            }

            if (!string.IsNullOrEmpty(ZipCode))
            {
                if (addressBuilder.Length > 0)
                    addressBuilder.Append(", ");
                addressBuilder.Append(ZipCode);
            }

            return addressBuilder.ToString();
        }
    }
}