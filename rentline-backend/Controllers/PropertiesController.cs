
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rentline_backend.Data;
using rentline_backend.Domain;
using rentline_backend.DTOs;
using rentline_backend.Services;
using System.Security.Claims;

namespace rentline_backend.Controllers;

[ApiController]
[Route("api/properties")]
public class PropertiesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly CloudinaryService _cloud;
    public PropertiesController(AppDbContext db, CloudinaryService cloud) { _db = db; _cloud = cloud; }

    [HttpGet]
    [Authorize(Policy = "OrgMember")]
    public async Task<IActionResult> List()
    {
        var data = await _db.Properties.Include(p => p.Images).ToListAsync();
        return Ok(data);
    }

    [HttpPost]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest req)
    {
        var orgId = Guid.Parse(User.FindFirstValue("orgId")!);
        var p = new Property { Id = Guid.NewGuid(), OrganizationId = orgId, Name = req.Name, Street=req.Street, City=req.City, State=req.State, PostalCode=req.PostalCode, Country=req.Country };
        _db.Properties.Add(p);
        await _db.SaveChangesAsync();
        return Ok(p);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyRequest req)
    {
        var p = await _db.Properties.FindAsync(id);
        if (p == null) return NotFound();
        p.Name = req.Name; p.Street=req.Street; p.City=req.City; p.State=req.State; p.PostalCode=req.PostalCode; p.Country=req.Country;
        await _db.SaveChangesAsync();
        return Ok(p);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var p = await _db.Properties.FindAsync(id);
        if (p == null) return NotFound();
        _db.Properties.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:guid}/images")]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
    {
        var p = await _db.Properties.FindAsync(id);
        if (p == null) return NotFound();
        using var s = file.OpenReadStream();
        var url = await _cloud.UploadAsync(s, file.FileName, "rentline/properties");
        var img = new PropertyImage { Id = Guid.NewGuid(), OrganizationId = p.OrganizationId, PropertyId = p.Id, Url = url };
        _db.PropertyImages.Add(img);
        await _db.SaveChangesAsync();
        return Ok(img);
    }
}
