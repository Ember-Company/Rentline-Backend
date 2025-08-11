namespace rentline_backend.Domain.Entities
{
    public class UserSetting : IMultiTenant
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public string Language { get; set; } = "en";
        public string Theme { get; set; } = "light";
        public AppUser? User { get; set; }
    }
}
