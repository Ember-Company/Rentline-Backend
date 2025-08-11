using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rentline_backend.Domain.Common;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     Represents a binding agreement between a landlord and a tenant
    ///     for a specific unit. Leases track the tenant, duration and rent
    ///     amount. Overlapping leases for the same unit should be
    ///     prevented at the application level.
    /// </summary>
    public class Lease : OrgEntity
    {
        [Required]
        public Guid UnitId { get; set; }

        /// <summary>
        ///     The user ID of the tenant occupying the unit. The
        ///     corresponding AppUser should have a Tenant role.
        /// </summary>
        [Required]
        public Guid TenantUserId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     Monthly rent amount for this lease. Use decimal for
        ///     monetary values to avoid floating point issues.
        /// </summary>
        [Column(TypeName = "money")]
        public decimal MonthlyRent { get; set; }

        /// <summary>
        ///     Optional currency code (ISO 4217). If null, defaults to
        ///     the currency of the parent property.
        /// </summary>
        [MaxLength(3)]
        public string? Currency { get; set; }

        /// <summary>
        ///     Indicates whether the lease is active, terminated or
        ///     completed. This can be derived by comparing the end
        ///     date with the current date; provided here for clarity.
        /// </summary>
        public bool IsActive => StartDate <= DateTime.UtcNow && EndDate >= DateTime.UtcNow;

        public Unit? Unit { get; set; }
        public AppUser? TenantUser { get; set; }
    }
}