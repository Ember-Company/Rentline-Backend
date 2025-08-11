using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rentline_backend.Domain.Enums;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for updating an existing property. All
    ///     properties are optional; only supplied values will be
    ///     updated.
    /// </summary>
    public class UpdatePropertyRequest
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(200)]
        public string? Street { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public PropertyType? Type { get; set; }
        public DateTime? PurchaseDate { get; set; }
        [Column(TypeName = "money")]
        public decimal? PurchasePrice { get; set; }
        public int? YearBuilt { get; set; }
        public decimal? LotSizeSqm { get; set; }
    }
}