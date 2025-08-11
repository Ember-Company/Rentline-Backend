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
    public class UnitInspectionsController : ControllerBase
    {
        private readonly RentlineDbContext _db;
        public UnitInspectionsController(RentlineDbContext db)
        {
            _db = db;
        }

        // GET: /api/units/{unitId}/inspections
        [HttpGet("api/units/{unitId}/inspections")]
        public async Task<IActionResult> GetInspections([FromRoute] Guid unitId)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var inspections = await _db.UnitInspections
                .Where(i => i.UnitId == unitId && i.OrgId == orgId)
                .Include(i => i.Issues)
                .Include(i => i.Photos)
                .ToListAsync();
            return Ok(inspections);
        }

        // POST: /api/units/{unitId}/inspections
        [HttpPost("api/units/{unitId}/inspections")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> CreateInspection([FromRoute] Guid unitId, [FromBody] CreateUnitInspectionRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            if (request.UnitId != Guid.Empty && request.UnitId != unitId)
                return BadRequest("UnitId mismatch between URL and body.");
            var unit = await _db.Units.FirstOrDefaultAsync(u => u.Id == unitId && u.OrgId == orgId);
            if (unit == null)
                return NotFound("Unit not found");
            var inspection = new UnitInspection
            {
                OrgId = orgId,
                UnitId = unitId,
                Date = request.Date,
                InspectorName = request.InspectorName,
                Passed = request.Passed,
                Notes = request.Notes
            };
            _db.UnitInspections.Add(inspection);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetInspections), new { unitId = unitId }, inspection);
        }

        // GET: /api/inspections/{id}
        [HttpGet("api/inspections/{id}")]
        public async Task<IActionResult> GetInspection([FromRoute] Guid id)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var inspection = await _db.UnitInspections
                .Include(i => i.Issues)
                .Include(i => i.Photos)
                .FirstOrDefaultAsync(i => i.Id == id && i.OrgId == orgId);
            if (inspection == null)
                return NotFound();
            return Ok(inspection);
        }

        // PUT: /api/inspections/{id}
        [HttpPut("api/inspections/{id}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> UpdateInspection([FromRoute] Guid id, [FromBody] UpdateUnitInspectionRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var inspection = await _db.UnitInspections.FirstOrDefaultAsync(i => i.Id == id && i.OrgId == orgId);
            if (inspection == null)
                return NotFound();
            if (request.Date != null) inspection.Date = request.Date.Value;
            if (request.InspectorName != null) inspection.InspectorName = request.InspectorName;
            if (request.Passed != null) inspection.Passed = request.Passed.Value;
            if (request.Notes != null) inspection.Notes = request.Notes;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/inspections/{id}
        [HttpDelete("api/inspections/{id}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> DeleteInspection([FromRoute] Guid id)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var inspection = await _db.UnitInspections.FirstOrDefaultAsync(i => i.Id == id && i.OrgId == orgId);
            if (inspection == null)
                return NotFound();
            // Cascade deletion of issues and photos is configured via EF
            _db.UnitInspections.Remove(inspection);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // POST: /api/inspections/{id}/issues
        [HttpPost("api/inspections/{id}/issues")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> AddIssue([FromRoute] Guid id, [FromBody] CreateInspectionIssueRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            // Ensure path param and body agree
            if (request.InspectionId != Guid.Empty && request.InspectionId != id)
                return BadRequest("InspectionId mismatch between URL and body.");
            var inspection = await _db.UnitInspections.FirstOrDefaultAsync(i => i.Id == id && i.OrgId == orgId);
            if (inspection == null)
                return NotFound("Inspection not found");
            var issue = new UnitInspectionIssue
            {
                UnitInspectionId = id,
                Description = request.Description,
                Severity = request.Severity,
                Resolved = request.Resolved
            };
            _db.UnitInspectionIssues.Add(issue);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetInspection), new { id = id }, issue);
        }

        // PUT: /api/issues/{issueId}
        [HttpPut("api/issues/{issueId}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> UpdateIssue([FromRoute] Guid issueId, [FromBody] UpdateInspectionIssueRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var issue = await _db.UnitInspectionIssues
                .Include(i => i.Inspection)
                .FirstOrDefaultAsync(ii => ii.Id == issueId && ii.Inspection != null && ii.Inspection.OrgId == orgId);
            if (issue == null)
                return NotFound();
            if (request.Description != null) issue.Description = request.Description;
            if (request.Severity != null) issue.Severity = request.Severity.Value;
            if (request.Resolved != null) issue.Resolved = request.Resolved.Value;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/issues/{issueId}
        [HttpDelete("api/issues/{issueId}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> DeleteIssue([FromRoute] Guid issueId)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var issue = await _db.UnitInspectionIssues
                .Include(i => i.Inspection)
                .FirstOrDefaultAsync(ii => ii.Id == issueId && ii.Inspection != null && ii.Inspection.OrgId == orgId);
            if (issue == null)
                return NotFound();
            _db.UnitInspectionIssues.Remove(issue);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // POST: /api/inspections/{id}/photos
        [HttpPost("api/inspections/{id}/photos")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> AddPhoto([FromRoute] Guid id, [FromBody] UnitInspectionPhoto photo)
        {
            // This placeholder expects the client to send the photo URL and caption
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var inspection = await _db.UnitInspections.FirstOrDefaultAsync(i => i.Id == id && i.OrgId == orgId);
            if (inspection == null)
                return NotFound("Inspection not found");
            photo.Id = Guid.NewGuid();
            photo.UnitInspectionId = id;
            _db.UnitInspectionPhotos.Add(photo);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetInspection), new { id = id }, photo);
        }

        // DELETE: /api/photos/{photoId}
        [HttpDelete("api/photos/{photoId}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> DeletePhoto([FromRoute] Guid photoId)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var photo = await _db.UnitInspectionPhotos
                .Include(p => p.Inspection)
                .FirstOrDefaultAsync(p => p.Id == photoId && p.Inspection != null && p.Inspection.OrgId == orgId);
            if (photo == null)
                return NotFound();
            _db.UnitInspectionPhotos.Remove(photo);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}