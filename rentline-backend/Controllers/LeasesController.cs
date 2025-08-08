
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rentline_backend.Data;
using rentline_backend.Domain;
using rentline_backend.DTOs;

namespace rentline_backend.Controllers;

[ApiController]
[Route("api/leases")]
public class LeasesController : ControllerBase
{
    private readonly AppDbContext _db;
    public LeasesController(AppDbContext db) { _db = db; }

    [HttpGet("by-unit/{unitId:guid}")]
    [Authorize(Policy = "OrgMember")]
    public async Task<IActionResult> ByUnit(Guid unitId)
    {
        var list = await _db.Leases.Where(l => l.UnitId == unitId).ToListAsync();
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> Create([FromBody] CreateLeaseRequest req)
    {
        // prevent overlapping leases per unit
        var overlap = await _db.Leases.AnyAsync(l => l.UnitId == req.UnitId && l.Status != LeaseStatus.Ended && req.StartDate <= l.EndDate && req.EndDate >= l.StartDate);
        if (overlap) return Conflict("Overlapping lease for unit");
        var orgId = (await _db.Units.Where(u => u.Id == req.UnitId).Select(u => u.OrganizationId).FirstAsync());
        var lease = new Lease { Id = Guid.NewGuid(), OrganizationId = orgId, UnitId = req.UnitId, TenantUserId = req.TenantUserId, StartDate=req.StartDate, EndDate=req.EndDate, MonthlyRent=req.MonthlyRent, Status=LeaseStatus.Pending };
        _db.Leases.Add(lease);
        await _db.SaveChangesAsync();
        return Ok(lease);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLeaseRequest req)
    {
        var l = await _db.Leases.FindAsync(id);
        if (l == null) return NotFound();
        l.UnitId = req.UnitId; l.TenantUserId = req.TenantUserId; l.StartDate=req.StartDate; l.EndDate=req.EndDate; l.MonthlyRent=req.MonthlyRent;
        if (Enum.TryParse<LeaseStatus>(req.Status, true, out var st)) l.Status = st;
        await _db.SaveChangesAsync();
        return Ok(l);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var l = await _db.Leases.FindAsync(id);
        if (l == null) return NotFound();
        _db.Leases.Remove(l);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
