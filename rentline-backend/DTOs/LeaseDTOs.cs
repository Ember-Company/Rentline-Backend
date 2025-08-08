
namespace rentline_backend.DTOs;

public class CreateLeaseRequest
{
    public System.Guid UnitId { get; set; }
    public System.Guid TenantUserId { get; set; }
    public System.DateTime StartDate { get; set; }
    public System.DateTime EndDate { get; set; }
    public decimal MonthlyRent { get; set; }
}

public class UpdateLeaseRequest : CreateLeaseRequest
{
    public string Status { get; set; } = "Pending"; // Pending/Active/Ended
}
