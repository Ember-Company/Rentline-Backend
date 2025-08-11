using System;
using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Common;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     Represents an individual key associated with a unit. Keys can
    ///     represent physical keys, fobs or access codes. Use this entity to
    ///     track who currently holds a key and when it was last issued.
    /// </summary>
    public class UnitKey : OrgEntity
    {
        [Required]
        public Guid UnitId { get; set; }

        /// <summary>
        ///     A label for this key (e.g. "Front door", "Mailbox", code number).
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Label { get; set; } = default!;

        /// <summary>
        ///     A unique code or identifier for this key. For physical keys this
        ///     might correspond to a stamped number; for digital locks this
        ///     could be an access code.
        /// </summary>
        [MaxLength(100)]
        public string? Code { get; set; }

        /// <summary>
        ///     Date and time when this key was issued to the current holder.
        /// </summary>
        public DateTime? IssuedAt { get; set; }

        /// <summary>
        ///     Optional user identifier for the current key holder (e.g. tenant).
        /// </summary>
        public Guid? HolderUserId { get; set; }

        public Unit? Unit { get; set; }
    }
}