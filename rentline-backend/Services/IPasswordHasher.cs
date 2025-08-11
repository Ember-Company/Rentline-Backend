namespace rentline_backend.Services
{
    /// <summary>
    ///     Abstraction for password hashing. Use a strong hashing
    ///     algorithm (e.g. BCrypt) to store user credentials securely.
    /// </summary>
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}