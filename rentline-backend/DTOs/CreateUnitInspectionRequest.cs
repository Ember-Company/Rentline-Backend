using System;
using System.ComponentModel.DataAnnotations;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for scheduling or recording a unit inspection.
    /// </summary>
    public class CreateUnitInspectionRequest
    {
        [Required]
        public Guid UnitId { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(200)]
        public string InspectorName { get; set; } = default!;

        public bool Passed { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}