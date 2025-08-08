
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rentline_backend.Data;
using rentline_backend.Domain;

namespace rentline_backend.Controllers;

[ApiController]
[Route("api/tenants")]
[Authorize(Policy = "OwnerOrManager")]
public class TenantsController : ControllerBase
{
    private readonly AppDbContext _db;
    public TenantsController(AppDbContext db) { _db = db; }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var tenants = await _db.Users.Where(u => u.Role == UserRole.Tenant).ToListAsync();
        return Ok(tenants);
    }
}
