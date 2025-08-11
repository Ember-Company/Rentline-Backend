using System;
using System.ComponentModel.DataAnnotations;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for updating an existing unit inspection. All
    ///     fields are optional.
    /// </summary>
    public class UpdateUnitInspectionRequest
    {
        public DateTime? Date { get; set; }
        [MaxLength(200)]
        public string? InspectorName { get; set; }
        public bool? Passed { get; set; }
        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}