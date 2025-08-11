using System;
using System.ComponentModel.DataAnnotations;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for updating an existing unit key. All fields
    ///     are optional.
    /// </summary>
    public class UpdateUnitKeyRequest
    {
        [MaxLength(100)]
        public string? Label { get; set; }
        [MaxLength(100)]
        public string? Code { get; set; }
        public DateTime? IssuedAt { get; set; }
        public Guid? HolderUserId { get; set; }
    }
}