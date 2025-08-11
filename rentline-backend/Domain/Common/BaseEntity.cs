using System;
using System.ComponentModel.DataAnnotations;

namespace rentline_backend.Domain.Common
{
    /// <summary>
    ///     Base class for all entities. Provides an ID and timestamps for
    ///     auditing purposes. Entities deriving from this type should use
    ///     <see cref="Guid"/> identifiers to ensure uniqueness across
    ///     tenants and deployments.
    /// </summary>
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        ///     The UTC date and time when the entity was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        ///     The UTC date and time when the entity was last modified.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        
    }
}