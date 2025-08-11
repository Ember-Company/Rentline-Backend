using System;
using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Enums;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for updating an existing equipment item. All
    ///     fields are optional.
    /// </summary>
    public class UpdateUnitEquipmentRequest
    {
        [MaxLength(100)]
        public string? Name { get; set; }
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
        public EquipmentCondition? Condition { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}