using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rentline_backend.DTOs;
using rentline_backend.Domain.Entities;
using rentline_backend.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rentline_backend.Controllers
{
    [ApiController]
    [Authorize(Policy = "OrgMember")]
    [Route("api/leases")]
    public class LeasesController : ControllerBase
    {
        private readonly RentlineDbContext _db;
        public LeasesController(RentlineDbContext db)
        {
            _db = db;
        }

        // GET: /api/leases/by-unit/{unitId}
        [HttpGet("by-unit/{unitId}")]
        public async Task<IActionResult> GetLeasesByUnit([FromRoute] Guid unitId)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var leases = await _db.Leases
                .Where(l => l.UnitId == unitId && l.OrgId == orgId)
                .ToListAsync();
            return Ok(leases);
        }

        // POST: /api/leases
        [HttpPost]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> CreateLease([FromBody] CreateLeaseRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            // Validate unit belongs to org
            var unit = await _db.Units.FirstOrDefaultAsync(u => u.Id == request.UnitId && u.OrgId == orgId);
            if (unit == null)
                return NotFound("Unit not found");
            // Prevent overlapping leases for the same unit
            var overlaps = await _db.Leases.AnyAsync(l => l.UnitId == request.UnitId && l.OrgId == orgId &&
                (request.StartDate <= l.EndDate && request.EndDate >= l.StartDate));
            if (overlaps)
                return Conflict("Lease dates overlap an existing lease.");
            // Create lease
            var lease = new Lease
            {
                OrgId = orgId,
                UnitId = request.UnitId,
                TenantUserId = request.TenantUserId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                MonthlyRent = request.MonthlyRent,
                Currency = request.Currency
            };
            _db.Leases.Add(lease);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLeasesByUnit), new { unitId = request.UnitId }, lease);
        }

        // PUT: /api/leases/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> UpdateLease([FromRoute] Guid id, [FromBody] UpdateLeaseRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var lease = await _db.Leases.FirstOrDefaultAsync(l => l.Id == id && l.OrgId == orgId);
            if (lease == null)
                return NotFound();
            if (request.TenantUserId != null) lease.TenantUserId = request.TenantUserId.Value;
            if (request.StartDate != null) lease.StartDate = request.StartDate.Value;
            if (request.EndDate != null) lease.EndDate = request.EndDate.Value;
            if (request.MonthlyRent != null) lease.MonthlyRent = request.MonthlyRent.Value;
            if (request.Currency != null) lease.Currency = request.Currency;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/leases/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> DeleteLease([FromRoute] Guid id)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var lease = await _db.Leases.FirstOrDefaultAsync(l => l.Id == id && l.OrgId == orgId);
            if (lease == null)
                return NotFound();
            _db.Leases.Remove(lease);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}