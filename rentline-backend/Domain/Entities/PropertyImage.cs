using rentline_backend.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     Represents an image associated with a property. Stored in Cloudinary
    ///     or similar and referenced by URL. Keeping this as a separate
    ///     entity allows multiple images per property and metadata (e.g. label).
    /// </summary>
    public class PropertyImage : OrgEntity
    {
        [Required]
        public Guid PropertyId { get; set; }

        /// <summary>
        ///     The URL of the uploaded image (e.g. Cloudinary). This string
        ///     should include any transformations required by the frontend.
        /// </summary>
        [Required]
        public string Url { get; set; } = default!;

        /// <summary>
        ///     Optional descriptive label (e.g. "Front elevation").
        /// </summary>
        [MaxLength(100)]
        public string? Label { get; set; }

        /// <summary>
        ///     Navigation property to the parent property.
        /// </summary>
        public Property? Property { get; set; }
    }
}