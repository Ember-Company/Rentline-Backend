using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rentline_backend.Domain.Enums;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for creating a new unit under a property.
    /// </summary>
    public class CreateUnitRequest
    {
        [Required]
        public Guid PropertyId { get; set; }

        [Required]
        [MaxLength(50)]
        public string UnitNumber { get; set; } = default!;

        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public decimal? AreaSqm { get; set; }

        [Column(TypeName = "money")]
        public decimal? RentAmount { get; set; }

        [MaxLength(3)]
        public string? Currency { get; set; }

        public UnitType UnitType { get; set; } = UnitType.Studio;
        public UnitStatus Status { get; set; } = UnitStatus.Vacant;

        public Guid? TenantUserId { get; set; }

        [Column(TypeName = "money")]
        public decimal? MarketRent { get; set; }

        [Column(TypeName = "money")]
        public decimal? DepositAmount { get; set; }

        [MaxLength(3)]
        public string? DepositCurrency { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}