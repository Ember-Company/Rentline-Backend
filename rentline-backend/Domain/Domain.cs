
namespace rentline_backend.Domain;

public interface IMultiTenant
{
    Guid OrganizationId { get; set; }
}

public enum OrgType { Landlord, Agency }

public enum UserRole { Landlord, AgencyAdmin, Manager, Maintenance, Tenant, Viewer }

public class Organization : IMultiTenant
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; } // self for filter
    public string Name { get; set; } = default!;
    public OrgType Type { get; set; }
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}

public class AppUser : IMultiTenant
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public UserRole Role { get; set; }
    public bool Active { get; set; } = true;
}

public class Property : IMultiTenant
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; }
    public string Name { get; set; } = default!;
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public ICollection<Unit> Units { get; set; } = new List<Unit>();
    public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
}

public class PropertyImage : IMultiTenant
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = default!;
    public string Url { get; set; } = default!;
    public string? Caption { get; set; }
}

public class Unit : IMultiTenant
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = default!;
    public string UnitNumber { get; set; } = default!;
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public decimal? AreaSqm { get; set; }
    public decimal? RentAmount { get; set; }
    public string Currency { get; set; } = "BRL";
}

public enum LeaseStatus { Pending, Active, Ended }

public class Lease : IMultiTenant
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid UnitId { get; set; }
    public Unit Unit { get; set; } = default!;
    public Guid TenantUserId { get; set; }
    public AppUser Tenant { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal MonthlyRent { get; set; }
    public LeaseStatus Status { get; set; } = LeaseStatus.Pending;
}

public class MaintenanceRequest : IMultiTenant
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid UnitId { get; set; }
    public Unit Unit { get; set; } = default!;
    public Guid CreatedByUserId { get; set; }
    public AppUser CreatedBy { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Status { get; set; } = "Open"; // Open, InProgress, Resolved, Closed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum InviteStatus { Pending, Accepted, Canceled, Expired }

public class Invite : IMultiTenant
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Email { get; set; } = default!;
    public UserRole Role { get; set; } = UserRole.Tenant;
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public InviteStatus Status { get; set; } = InviteStatus.Pending;
}
