
namespace rentline_backend.DTOs;

public class CreateMaintenanceRequest
{
    public System.Guid UnitId { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
}
