
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
public class PropertiesController(AppDbContext db, CloudinaryService cloud, PropertyService _service) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "OrgMember")]
    public async Task<IActionResult> List()
    {
        var data = await db.Properties.Include(p => p.Images).ToListAsync();
        return Ok(data);
    }

    [HttpPost]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest req)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdStr, out var currentUserId))
            return Unauthorized("Invalid user ID.");

        var roleClaim = User.FindFirstValue(ClaimTypes.Role)
              ?? User.FindFirstValue("role");

        if (!Enum.TryParse<UserRole>(roleClaim, ignoreCase: true, out UserRole currentUserRole))
        {
            currentUserRole = UserRole.Viewer;
        }

        var orgIdStr = User.FindFirstValue("orgId");
        if (!Guid.TryParse(orgIdStr, out var orgId))
            return Forbid("No organisation ID present.");
        

        var org = await db.Organizations.FirstOrDefaultAsync(o => o.Id == orgId);
        if (org == null)
            return NotFound("Organisation not found.");

        var result = await _service.CreatePropertyAsync(req, currentUserId, currentUserRole, org);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyRequest req)
    {
        var p = await db.Properties.FindAsync(id);
        if (p == null) return NotFound();
        p.Name = req.Name; p.Street=req.Street; p.City=req.City; p.State=req.State; p.PostalCode=req.PostalCode; p.Country=req.Country;
        await db.SaveChangesAsync();
        return Ok(p);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var p = await db.Properties.FindAsync(id);
        if (p == null) return NotFound();
        db.Properties.Remove(p);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:guid}/images")]
    [Authorize(Policy = "OwnerOrManager")]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
    {
        var p = await db.Properties.FindAsync(id);
        if (p == null) return NotFound();
        using var s = file.OpenReadStream();
        var url = await cloud.UploadAsync(s, file.FileName, "rentline/properties");
        var img = new PropertyImage { Id = Guid.NewGuid(), OrganizationId = p.OrganizationId, PropertyId = p.Id, Url = url };
        db.PropertyImages.Add(img);
        await db.SaveChangesAsync();
        return Ok(img);
    }
}
