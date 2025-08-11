using System;
using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Common;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     Represents a document or file associated with a property,
    ///     unit or lease. Documents might include tenancy agreements,
    ///     inspection reports, invoices or other relevant files. Files
    ///     should be stored externally (e.g. S3, Cloudinary) and
    ///     referenced via the Url.
    /// </summary>
    public class Document : OrgEntity
    {
        /// <summary>
        ///     Optional associated property. A document may be linked to
        ///     either a property, a unit, a lease or none (for general
        ///     organisation files).
        /// </summary>
        public Guid? PropertyId { get; set; }

        public Guid? UnitId { get; set; }
        public Guid? LeaseId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        [Required]
        public string Url { get; set; } = default!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public Guid? UploadedByUserId { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Property? Property { get; set; }
        public Unit? Unit { get; set; }
        public Lease? Lease { get; set; }
        public AppUser? UploadedByUser { get; set; }
    }
}