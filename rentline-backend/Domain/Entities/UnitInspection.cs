using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Common;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     Represents a scheduled or ad‑hoc inspection of a unit. Inspections
    ///     capture the date, inspector and any notes or issues discovered.
    /// </summary>
    public class UnitInspection : OrgEntity
    {
        [Required]
        public Guid UnitId { get; set; }

        /// <summary>
        ///     The date the inspection took place.
        /// </summary>
        public DateTime Date { get; set; } = DateTime.UtcNow;

        /// <summary>
        ///     The name of the person who performed the inspection.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string InspectorName { get; set; } = default!;

        /// <summary>
        ///     Indicates whether the inspection passed without issues.
        /// </summary>
        public bool Passed { get; set; }

        /// <summary>
        ///     Additional comments or observations.
        /// </summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        public ICollection<UnitInspectionIssue> Issues { get; set; } = new List<UnitInspectionIssue>();
        public ICollection<UnitInspectionPhoto> Photos { get; set; } = new List<UnitInspectionPhoto>();
        public Unit? Unit { get; set; }
    }
}