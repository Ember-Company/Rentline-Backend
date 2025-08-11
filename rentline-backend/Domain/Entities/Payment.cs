using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rentline_backend.Domain.Common;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     Represents a payment made against a lease. Payments are
    ///     typically recorded by the landlord to track rent collection.
    /// </summary>
    public class Payment : OrgEntity
    {
        [Required]
        public Guid LeaseId { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        ///     Method used for the payment (e.g. bank transfer, cash, check).
        /// </summary>
        [MaxLength(50)]
        public string? Method { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public Lease? Lease { get; set; }
    }
}