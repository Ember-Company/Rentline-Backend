
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rentline_backend.Data;
using rentline_backend.Domain;
using rentline_backend.DTOs;

namespace rentline_backend.Controllers;

[ApiController]
[Route("api/units")]
[Authorize(Policy = "OrgMember")]
public class UnitsController : ControllerBase
{
    private readonly AppDbContext _db;
    public UnitsController(AppDbContext db) { _db = db; }

    [HttpGet("by-property/{propertyId:guid}")]
    public async Task<IActionResult> ByProperty(Guid propertyId)
    {
        var list = await _db.Units.Where(u => u.PropertyId == propertyId).ToListAsync();
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> Create([FromBody] CreateUnitRequest req)
    {
        var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
        var u = new Unit { Id = Guid.NewGuid(), OrganizationId = orgId, PropertyId = req.PropertyId, UnitNumber = req.UnitNumber, Bedrooms=req.Bedrooms, Bathrooms=req.Bathrooms, AreaSqm=req.AreaSqm, RentAmount=req.RentAmount, Currency=req.Currency };
        _db.Units.Add(u);
        await _db.SaveChangesAsync();
        return Ok(u);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnitRequest req)
    {
        var u = await _db.Units.FindAsync(id);
        if (u == null) return NotFound();
        u.UnitNumber=req.UnitNumber; u.Bedrooms=req.Bedrooms; u.Bathrooms=req.Bathrooms; u.AreaSqm=req.AreaSqm; u.RentAmount=req.RentAmount; u.Currency=req.Currency;
        await _db.SaveChangesAsync();
        return Ok(u);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var u = await _db.Units.FindAsync(id);
        if (u == null) return NotFound();
        _db.Units.Remove(u);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
