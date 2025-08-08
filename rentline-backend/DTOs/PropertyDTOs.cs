
namespace rentline_backend.DTOs;

public class PropertyDto
{
    public Guid Id { get; set; }
    public String Name { get; set; }
    public String Street { get; set; }
    public String City { get; set; }
    public String State { get; set; }
    public String PostalCode { get; set; }
    public String Country { get; set; }
    public Guid? OwnerUserId { get; set; }
}

public class CreatePropertyRequest
{
    public string Name { get; set; } = default!;
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public Guid? OwnerUserId { get; set; }

}

public class UpdatePropertyRequest : CreatePropertyRequest {}
