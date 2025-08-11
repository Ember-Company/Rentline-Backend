using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Enums;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for updating an inspection issue. All fields
    ///     are optional.
    /// </summary>
    public class UpdateInspectionIssueRequest
    {
        [MaxLength(500)]
        public string? Description { get; set; }
        public InspectionIssueSeverity? Severity { get; set; }
        public bool? Resolved { get; set; }
    }
}