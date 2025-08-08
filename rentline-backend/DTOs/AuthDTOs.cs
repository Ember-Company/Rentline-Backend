
namespace rentline_backend.DTOs;

public class RegisterOrgRequest
{
    public string OrgName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string OrgType { get; set; } = "Landlord"; // Landlord or Agency
}

public class LoginRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class AcceptInviteRequest
{
    public string Token { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Password { get; set; } = default!;
}
