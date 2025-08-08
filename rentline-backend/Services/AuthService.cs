
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using rentline_backend.Domain;
using rentline_backend.Data;

namespace rentline_backend.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _cfg;

    public AuthService(AppDbContext db, IConfiguration cfg)
    {
        _db = db; _cfg = cfg;
    }

    public async Task<(AppUser user, Organization org)> RegisterOrgAsync(string orgName, OrgType type, string email, string password, string displayName)
    {
        // create organization
        var org = new Organization { Id = Guid.NewGuid(), Name = orgName, Type = type };
        org.OrganizationId = org.Id;
        _db.Organizations.Add(org);

        // create user as landlord or agency admin
        var role = type == OrgType.Landlord ? UserRole.Landlord : UserRole.AgencyAdmin;
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            OrganizationId = org.Id,
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            DisplayName = displayName,
            Role = role
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return (user, org);
    }

    public async Task<AppUser?> ValidateUserAsync(string email, string password)
    {
        var u = await _db.Users.FirstOrDefaultAsync(x => x.Email == email.ToLower());
        if (u == null) return null;
        if (!BCrypt.Net.BCrypt.Verify(password, u.PasswordHash)) return null;
        return u;
    }

    public string IssueJwt(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("orgId", user.OrganizationId.ToString())
        };
        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
