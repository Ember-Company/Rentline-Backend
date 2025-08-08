
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rentline_backend.DTOs;
using rentline_backend.Services;
using System.Security.Claims;

namespace rentline_backend.Controllers;

[ApiController]
[Route("api/invites")]
public class InvitesController : ControllerBase
{
    private readonly InviteService _svc;
    public InvitesController(InviteService svc) { _svc = svc; }

    [HttpPost]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> Send([FromBody] SendInviteRequest req)
    {
        var orgId = Guid.Parse(User.FindFirstValue("orgId")!);
        var inv = await _svc.CreateTenantInviteAsync(orgId, req.Email);
        return Ok(new { token = inv.Token, email = inv.Email, expiresAt = inv.ExpiresAt });
    }
}
