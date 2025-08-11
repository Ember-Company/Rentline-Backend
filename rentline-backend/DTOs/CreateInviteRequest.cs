using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Enums;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for creating an invitation for a new user.
    /// </summary>
    public class CreateInviteRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        public Role Role { get; set; } = Role.Tenant;

        /// <summary>
        ///     Number of days before the invitation expires. Defaults to 7.
        /// </summary>
        public int ExpiresInDays { get; set; } = 7;
    }
}