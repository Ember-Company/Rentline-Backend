namespace rentline_backend
{
    /// <summary>
    ///     Configuration settings for JSON Web Token generation. These
    ///     settings should be stored in appsettings.json and bound to
    ///     this class.
    /// </summary>
    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; } = 60;
    }
}