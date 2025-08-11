using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Enums;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for registering a new organisation and its first
    ///     admin user.
    /// </summary>
    public class RegisterOrgRequest
    {
        [Required]
        [MaxLength(200)]
        public string OrgName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = default!;

        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = default!;

        public OrgType OrgType { get; set; } = OrgType.Landlord;
    }
}