
namespace rentline_backend.DTOs;

public class CreatePropertyRequest
{
    public string Name { get; set; } = default!;
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}

public class UpdatePropertyRequest : CreatePropertyRequest {}
