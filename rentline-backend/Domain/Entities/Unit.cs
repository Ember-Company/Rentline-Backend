using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rentline_backend.Domain.Common;
using rentline_backend.Domain.Enums;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     Represents an individual rentable unit within a property. Units
    ///     capture information about their configuration, rent and
    ///     occupancy status, as well as associated keys, equipment and
    ///     inspections.
    /// </summary>
    public class Unit : OrgEntity
    {
        [Required]
        public Guid PropertyId { get; set; }

        /// <summary>
        ///     The unit identifier visible to tenants (e.g. "A‑101").
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UnitNumber { get; set; } = default!;

        /// <summary>
        ///     Optional number of bedrooms in this unit.
        /// </summary>
        public int? Bedrooms { get; set; }

        /// <summary>
        ///     Optional number of bathrooms in this unit.
        /// </summary>
        public int? Bathrooms { get; set; }

        /// <summary>
        ///     Total floor area in square metres. Nullable for storage units.
        /// </summary>
        public decimal? AreaSqm { get; set; }

        /// <summary>
        ///     The agreed rent amount per month (base amount). Use decimal for
        ///     precise monetary values. Currency can vary by property.
        /// </summary>
        [Column(TypeName = "money")]
        public decimal? RentAmount { get; set; }

        /// <summary>
        ///     ISO 4217 currency code (e.g. "BRL", "USD").
        /// </summary>
        [MaxLength(3)]
        public string? Currency { get; set; }

        /// <summary>
        ///     The classification of this unit (studio, 1 bedroom etc.).
        /// </summary>
        public UnitType UnitType { get; set; }

        /// <summary>
        ///     Current occupancy status.
        /// </summary>
        public UnitStatus Status { get; set; }

        /// <summary>
        ///     Optional external identifier for the current tenant (user ID).
        ///     The domain may model tenants as a separate entity; however
        ///     storing the ID here simplifies lookups and foreign key
        ///     relationships.
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        ///     Market rent suggestion (may differ from the current rent).
        /// </summary>
        [Column(TypeName = "money")]
        public decimal? MarketRent { get; set; }

        /// <summary>
        ///     Security deposit amount required for this unit. Nullable
        ///     because some markets or lease types may not require a
        ///     deposit.
        /// </summary>
        [Column(TypeName = "money")]
        public decimal? DepositAmount { get; set; }

        /// <summary>
        ///     ISO 4217 currency code for the deposit. If null, defaults
        ///     to the currency used for rent.
        /// </summary>
        [MaxLength(3)]
        public string? DepositCurrency { get; set; }

        /// <summary>
        ///     Free‑form notes about this unit.
        /// </summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }
        

        /// <summary>
        ///     Navigation property to the parent property.
        /// </summary>
        public Property? Property { get; set; }

        public ICollection<UnitKey> Keys { get; set; } = new List<UnitKey>();
        public ICollection<UnitEquipment> Equipment { get; set; } = new List<UnitEquipment>();
        public ICollection<UnitInspection> Inspections { get; set; } = new List<UnitInspection>();
    }
}