using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using rentline_backend.Domain.Common;
using rentline_backend.Domain.Enums;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     Represents a physical property that belongs to an organisation.
    ///     A property can contain multiple units and associated images.
    /// </summary>
    public class Property : OrgEntity
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

        /// <summary>
        ///     High‑level classification of this property. Defaults to
        ///     <see cref="PropertyType.Residential"/>. This value allows
        ///     landlords to group and analyse their portfolio by type.
        /// </summary>
        public PropertyType Type { get; set; } = PropertyType.Residential;

        /// <summary>
        ///     The date the property was purchased by the current owner or
        ///     organisation. Nullable to allow import of properties with
        ///     unknown acquisition dates.
        /// </summary>
        public DateTime? PurchaseDate { get; set; }

        /// <summary>
        ///     The purchase price paid for this property, expressed in
        ///     the local currency. Nullable to support legacy data.
        /// </summary>
        [Column(TypeName = "money")]
        public decimal? PurchasePrice { get; set; }

        /// <summary>
        ///     Year the property was built. Optional as some older
        ///     buildings may not have a precise year recorded.
        /// </summary>
        public int? YearBuilt { get; set; }

        /// <summary>
        ///     Total lot area in square metres. Useful for properties
        ///     comprising land (e.g. standalone houses or commercial
        ///     buildings). Nullable for apartments where common land size
        ///     isn't directly attributed to individual units.
        /// </summary>
        public decimal? LotSizeSqm { get; set; }

        /// <summary>
        ///     Units that belong to this property. This collection is loaded
        ///     eagerly via Include/ThenInclude when necessary.
        /// </summary>
        public ICollection<Unit> Units { get; set; } = new List<Unit>();

        /// <summary>
        ///     Images associated with this property (e.g. marketing photos).
        /// </summary>
        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    }
}