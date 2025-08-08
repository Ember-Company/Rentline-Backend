
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rentline_backend.Data;
using System.Security.Claims;

namespace rentline_backend.Controllers;

[ApiController]
[Route("api/org")]
[Authorize(Policy = "OrgMember")]
public class OrgController : ControllerBase
{
    private readonly AppDbContext _db;
    public OrgController(AppDbContext db) { _db = db; }


    [HttpGet("me")]
    public async Task<IActionResult> MyOrg()
    {
        var orgStr = User.FindFirstValue("orgId");
        if (!Guid.TryParse(orgStr, out var orgId)) return Forbid();

        var org = await _db.Organizations
            .Where(o => o.Id == orgId)
            .Include(o => o.Properties)
                .ThenInclude(p => p.Units)
            .Include(o => o.Properties)
                .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync();

        if (org == null) return NotFound();
        return Ok(org);
    }
}
