using System;
using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Common;
using rentline_backend.Domain.Enums;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     A specific issue discovered during an inspection. Issues include
    ///     a severity level and whether they have been resolved. Use
    ///     separate entities for issues to support multiple issues per
    ///     inspection.
    /// </summary>
    public class UnitInspectionIssue : BaseEntity
    {
        [Required]
        public Guid UnitInspectionId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = default!;

        public InspectionIssueSeverity Severity { get; set; } = InspectionIssueSeverity.Medium;

        /// <summary>
        ///     If true, this issue has been addressed.
        /// </summary>
        public bool Resolved { get; set; }

        public UnitInspection? Inspection { get; set; }
    }
}