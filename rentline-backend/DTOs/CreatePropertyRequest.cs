using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rentline_backend.Domain.Enums;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for creating a new property.
    /// </summary>
    public class CreatePropertyRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        [Required]
        [MaxLength(200)]
        public string Street { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = default!;

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; } = default!;

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = default!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public PropertyType Type { get; set; } = PropertyType.Residential;

        public DateTime? PurchaseDate { get; set; }

        [Column(TypeName = "money")]
        public decimal? PurchasePrice { get; set; }

        public int? YearBuilt { get; set; }
        public decimal? LotSizeSqm { get; set; }
    }
}