using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Common;
using rentline_backend.Domain.Enums;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     Represents a person using the system. Users belong to an
    ///     organisation and have a role which governs their access. A
    ///     hashed password is stored for authentication; never store
    ///     plaintext passwords.
    /// </summary>
    public class AppUser : OrgEntity
    {
        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = default!;

        [Required]
        public string PasswordHash { get; set; } = default!;

        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = default!;

        /// <summary>
        ///     The role assigned to this user, determining their
        ///     permissions.
        /// </summary>
        public Role Role { get; set; } = Role.Viewer;

        public bool IsEmailVerified { get; set; } = false;

        /// <summary>
        ///     Optional reference to a tenant record (if this user
        ///     represents a tenant in a lease). You can use the AppUser
        ///     directly as the tenant; this Id is provided for
        ///     denormalisation or referencing external identity providers.
        /// </summary>
        public Guid? TenantId { get; set; }

        public ICollection<Lease> Leases { get; set; } = new List<Lease>();
    }
}