using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for creating a new lease.
    /// </summary>
    public class CreateLeaseRequest
    {
        [Required]
        public Guid UnitId { get; set; }

        [Required]
        public Guid TenantUserId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Column(TypeName = "money")]
        public decimal MonthlyRent { get; set; }

        [MaxLength(3)]
        public string? Currency { get; set; }
    }
}