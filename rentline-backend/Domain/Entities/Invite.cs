using System;
using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Common;
using rentline_backend.Domain.Enums;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     Represents an invitation sent by an organisation to a
    ///     prospective user (usually a tenant). Invites contain a
    ///     one‑time token used for registration and expire after a
    ///     configured period.
    /// </summary>
    public class Invite : OrgEntity
    {
        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = default!;

        [Required]
        public string Token { get; set; } = default!;

        public Role Role { get; set; } = Role.Tenant;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }

        public bool Used { get; set; } = false;
    }
}