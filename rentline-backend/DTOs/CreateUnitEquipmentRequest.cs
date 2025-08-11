using System;
using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Enums;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for adding a piece of equipment to a unit.
    /// </summary>
    public class CreateUnitEquipmentRequest
    {
        [Required]
        public Guid UnitId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        [MaxLength(50)]
        public string? Category { get; set; }
        [MaxLength(100)]
        public string? Brand { get; set; }
        [MaxLength(100)]
        public string? Model { get; set; }
        [MaxLength(100)]
        public string? SerialNumber { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public EquipmentCondition Condition { get; set; } = EquipmentCondition.Good;
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}