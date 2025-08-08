
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rentline_backend.Data;
using rentline_backend.Domain;
using rentline_backend.DTOs;
using System.Security.Claims;

namespace rentline_backend.Controllers;

[ApiController]
[Route("api/maintenance")]
public class MaintenanceController : ControllerBase
{
    private readonly AppDbContext _db;
    public MaintenanceController(AppDbContext db) { _db = db; }

    [HttpGet]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> List()
    {
        var list = await _db.MaintenanceRequests.OrderByDescending(m => m.CreatedAt).ToListAsync();
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Policy = "TenantOnly")]
    public async Task<IActionResult> Create([FromBody] CreateMaintenanceRequest req)
    {
        var uid = Guid.Parse(User.FindFirstValue("sub")!);
        var orgId = Guid.Parse(User.FindFirstValue("orgId")!);
        var m = new MaintenanceRequest { Id = Guid.NewGuid(), OrganizationId = orgId, UnitId = req.UnitId, CreatedByUserId = uid, Title = req.Title, Description = req.Description };
        _db.MaintenanceRequests.Add(m);
        await _db.SaveChangesAsync();
        return Ok(m);
    }

    [HttpPost("{id:guid}/status")]
    [Authorize(Policy = "MaintenanceOrManager")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        var m = await _db.MaintenanceRequests.FindAsync(id);
        if (m == null) return NotFound();
        m.Status = status;
        await _db.SaveChangesAsync();
        return Ok(m);
    }
}
