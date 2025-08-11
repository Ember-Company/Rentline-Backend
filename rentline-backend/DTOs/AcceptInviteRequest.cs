using System.ComponentModel.DataAnnotations;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for accepting an invitation and registering as a
    ///     tenant or user. A token is supplied by the inviter.
    /// </summary>
    public class AcceptInviteRequest
    {
        [Required]
        public string Token { get; set; } = default!;

        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = default!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = default!;
    }
}