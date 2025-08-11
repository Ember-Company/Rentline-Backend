using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for updating an existing lease. All fields
    ///     optional.
    /// </summary>
    public class UpdateLeaseRequest
    {
        public Guid? TenantUserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Column(TypeName = "money")]
        public decimal? MonthlyRent { get; set; }
        [MaxLength(3)]
        public string? Currency { get; set; }
    }
}