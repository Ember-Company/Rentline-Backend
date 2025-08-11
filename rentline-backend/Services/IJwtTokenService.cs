using rentline_backend.Domain.Entities;

namespace rentline_backend.Services
{
    /// <summary>
    ///     Service for generating JSON Web Tokens for authenticated users.
    /// </summary>
    public interface IJwtTokenService
    {
        string GenerateToken(AppUser user);
    }
}