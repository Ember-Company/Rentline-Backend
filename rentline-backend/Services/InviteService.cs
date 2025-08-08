
using rentline_backend.Domain;
using rentline_backend.Data;

namespace rentline_backend.Services;

public class InviteService
{
    private readonly AppDbContext _db;
    public InviteService(AppDbContext db) { _db = db; }

    public async Task<Invite> CreateTenantInviteAsync(Guid orgId, string email)
    {
        var inv = new Invite
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            Email = email.Trim().ToLowerInvariant(),
            Role = UserRole.Tenant,
            Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("+","-").Replace("/","_").TrimEnd('='),
            ExpiresAt = DateTime.UtcNow.AddDays(14)
        };
        _db.Invites.Add(inv);
        await _db.SaveChangesAsync();
        return inv;
    }

    public async Task<(Invite? invite, string? error)> AcceptAsync(string token, string displayName, string password)
    {
        var inv = _db.Invites.FirstOrDefault(i => i.Token == token);
        if (inv == null) return (null, "Invalid token");
        if (inv.ExpiresAt < DateTime.UtcNow) { inv.Status = InviteStatus.Expired; await _db.SaveChangesAsync(); return (null, "Expired"); }
        inv.Status = InviteStatus.Accepted;
        inv.AcceptedAt = DateTime.UtcNow;

        // create tenant user
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            OrganizationId = inv.OrganizationId,
            Email = inv.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            DisplayName = displayName,
            Role = UserRole.Tenant
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return (inv, null);
    }
}
