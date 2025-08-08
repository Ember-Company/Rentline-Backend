
namespace rentline_backend.DTOs;

public class CreateUnitRequest
{
    public System.Guid PropertyId { get; set; }
    public string UnitNumber { get; set; } = default!;
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public decimal? AreaSqm { get; set; }
    public decimal? RentAmount { get; set; }
    public string Currency { get; set; } = "BRL";
}

public class UpdateUnitRequest : CreateUnitRequest {}
