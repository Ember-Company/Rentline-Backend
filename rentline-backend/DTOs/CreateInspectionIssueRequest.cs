using System;
using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Enums;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for adding an issue to an inspection.
    /// </summary>
    public class CreateInspectionIssueRequest
    {
        [Required]
        public Guid InspectionId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = default!;

        public InspectionIssueSeverity Severity { get; set; } = InspectionIssueSeverity.Medium;

        public bool Resolved { get; set; }
    }
}