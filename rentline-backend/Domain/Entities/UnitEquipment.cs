using System;
using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Common;
using rentline_backend.Domain.Enums;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     Represents a piece of equipment installed in a unit. Each item
    ///     records essential information such as make/model and warranty
    ///     dates to assist with asset management and maintenance planning.
    /// </summary>
    public class UnitEquipment : OrgEntity
    {
        [Required]
        public Guid UnitId { get; set; }

        /// <summary>
        ///     A human‑readable name (e.g. "Refrigerator", "HVAC").
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = default!;

        /// <summary>
        ///     Category or type of equipment (e.g. Appliance, Utility). This
        ///     string can be used to group equipment for reporting.
        /// </summary>
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

        public Unit? Unit { get; set; }
    }
}