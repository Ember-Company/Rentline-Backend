
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rentline_backend.Data;
using rentline_backend.Domain;
using rentline_backend.DTOs;
using rentline_backend.Services;

namespace rentline_backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly AuthService _auth;
    private readonly InviteService _invites;

    public AuthController(AppDbContext db, AuthService auth, InviteService invites)
    {
        _db = db; _auth = auth; _invites = invites;
    }

    [HttpPost("register-org")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterOrg([FromBody] RegisterOrgRequest req)
    {
        var type = req.OrgType.Equals("Agency", StringComparison.OrdinalIgnoreCase) ? OrgType.Agency : OrgType.Landlord;
        var (user, org) = await _auth.RegisterOrgAsync(req.OrgName, type, req.Email, req.Password, req.DisplayName);
        var jwt = _auth.IssueJwt(user);
        return Ok(new { token = jwt, orgId = org.Id, role = user.Role.ToString() });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var u = await _auth.ValidateUserAsync(req.Email, req.Password);
        if (u == null) return Unauthorized();
        var jwt = _auth.IssueJwt(u);
        return Ok(new { token = jwt, orgId = u.OrganizationId, role = u.Role.ToString() });
    }

    [HttpPost("accept-invite")]
    [AllowAnonymous]
    public async Task<IActionResult> AcceptInvite([FromBody] AcceptInviteRequest req)
    {
        var (inv, err) = await _invites.AcceptAsync(req.Token, req.DisplayName, req.Password);
        if (inv == null) return BadRequest(new { error = err });
        var user = await _db.Users.FirstAsync(u => u.Email == inv.Email);
        var jwt = new AuthService(_db, HttpContext.RequestServices.GetRequiredService<IConfiguration>()).IssueJwt(user);
        return Ok(new { token = jwt, orgId = user.OrganizationId, role = user.Role.ToString() });
    }
}
