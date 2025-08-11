using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rentline_backend.DTOs;
using rentline_backend.Domain.Entities;
using rentline_backend.Domain.Enums;
using rentline_backend.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rentline_backend.Controllers
{
    [ApiController]
    [Authorize(Policy = "OrgMember")]
    [Route("api/maintenance")]
    public class MaintenanceController : ControllerBase
    {
        private readonly RentlineDbContext _db;
        public MaintenanceController(RentlineDbContext db)
        {
            _db = db;
        }

        // GET: /api/maintenance
        [HttpGet]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> GetAll()
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var requests = await _db.MaintenanceRequests
                .Where(r => r.OrgId == orgId)
                .Include(r => r.Unit)
                .Include(r => r.CreatedByUser)
                .ToListAsync();
            return Ok(requests);
        }

        // POST: /api/maintenance
        [HttpPost]
        [Authorize(Policy = "TenantOnly")]
        public async Task<IActionResult> Create([FromBody] CreateMaintenanceRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            // Validate unit exists
            var unit = await _db.Units.FirstOrDefaultAsync(u => u.Id == request.UnitId && u.OrgId == orgId);
            if (unit == null)
                return NotFound("Unit not found");
            var mr = new MaintenanceRequest
            {
                OrgId = orgId,
                UnitId = request.UnitId,
                Title = request.Title,
                Description = request.Description,
                Status = MaintenanceStatus.Pending,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _db.MaintenanceRequests.Add(mr);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { }, mr);
        }

        // POST: /api/maintenance/{id}/status
        [HttpPost("{id}/status")]
        [Authorize(Policy = "MaintenanceOrManager")]
        public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateMaintenanceStatusRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var mr = await _db.MaintenanceRequests.FirstOrDefaultAsync(r => r.Id == id && r.OrgId == orgId);
            if (mr == null)
                return NotFound();
            mr.Status = request.Status;
            mr.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}