namespace rentline_backend.DTOs
{
    public class UserSettingsDto
    {
        public string Language { get; set; } = string.Empty;
        public string Theme { get; set; } = string.Empty;
    }

    public class UpdateUserSettingsRequest
    {
        public string? Language { get; set; }
        public string? Theme { get; set; }
    }

    public class OrganizationSettingsDto
    {
        public string DefaultLanguage { get; set; } = string.Empty;
        public string DefaultTheme { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
    }

    public class UpdateOrganizationSettingsRequest
    {
        public string? DefaultLanguage { get; set; }
        public string? DefaultTheme { get; set; }
        public string? Currency { get; set; }
    }
}
