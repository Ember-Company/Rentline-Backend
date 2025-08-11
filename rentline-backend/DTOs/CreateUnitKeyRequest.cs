using System;
using System.ComponentModel.DataAnnotations;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for creating a new key or access code for a unit.
    /// </summary>
    public class CreateUnitKeyRequest
    {
        [Required]
        public Guid UnitId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Label { get; set; } = default!;

        [MaxLength(100)]
        public string? Code { get; set; }

        public DateTime? IssuedAt { get; set; }
        public Guid? HolderUserId { get; set; }
    }
}