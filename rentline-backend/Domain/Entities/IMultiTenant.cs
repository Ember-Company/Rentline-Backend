namespace rentline_backend.Domain.Entities
{
    public interface IMultiTenant
    {
        Guid OrganizationId { get; set; }
    }
}
