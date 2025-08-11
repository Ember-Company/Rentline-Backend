namespace rentline_backend.Domain.Entities
{
    public class OrganizationSetting : IMultiTenant
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string DefaultLanguage { get; set; } = "en";
        public string DefaultTheme { get; set; } = "light";
        public string Currency { get; set; } = "BRL";
        public Organization? Organization { get; set; }
    }
}
