using BCrypt.Net;

namespace rentline_backend.Services
{
    /// <summary>
    ///     Implementation of <see cref="IPasswordHasher"/> using the
    ///     BCrypt algorithm for secure password storage. BCrypt automatically
    ///     handles salting and includes the salt as part of the generated
    ///     hash.
    /// </summary>
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}