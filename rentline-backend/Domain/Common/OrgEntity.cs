using System;

namespace rentline_backend.Domain.Common
{
    /// <summary>
    ///     Base class for multi‑tenant entities. Adds an OrgId property to
    ///     partition data by organization. Every entity that belongs to a
    ///     specific organisation should derive from this class instead of
    ///     <see cref="BaseEntity"/>.
    /// </summary>
    public abstract class OrgEntity : BaseEntity
    {
        /// <summary>
        ///     The identifier of the owning organisation. Used by global query
        ///     filters to restrict visibility and access to data.
        /// </summary>
        public Guid OrgId { get; set; }
    }
}