using System;
using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Common;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     A photo taken as part of an inspection. Stored by URL and
    ///     captioned to provide context. Keeping photos separate allows
    ///     multiple images per inspection without bloating the parent
    ///     entity.
    /// </summary>
    public class UnitInspectionPhoto : BaseEntity
    {
        [Required]
        public Guid UnitInspectionId { get; set; }

        [Required]
        public string Url { get; set; } = default!;

        [MaxLength(200)]
        public string? Caption { get; set; }

        public UnitInspection? Inspection { get; set; }
    }
}