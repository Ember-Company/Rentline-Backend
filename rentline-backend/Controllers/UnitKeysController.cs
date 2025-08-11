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
    public class UnitKeysController : ControllerBase
    {
        private readonly RentlineDbContext _db;
        public UnitKeysController(RentlineDbContext db)
        {
            _db = db;
        }

        // GET: /api/units/{unitId}/keys
        [HttpGet("api/units/{unitId}/keys")]
        public async Task<IActionResult> GetKeys([FromRoute] Guid unitId)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var keys = await _db.UnitKeys.Where(k => k.UnitId == unitId && k.OrgId == orgId).ToListAsync();
            return Ok(keys);
        }

        // POST: /api/units/{unitId}/keys
        [HttpPost("api/units/{unitId}/keys")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> CreateKey([FromRoute] Guid unitId, [FromBody] CreateUnitKeyRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            // Ensure path param and body agree
            if (request.UnitId != Guid.Empty && request.UnitId != unitId)
                return BadRequest("UnitId mismatch between URL and body.");
            // Validate unit existence
            var unit = await _db.Units.FirstOrDefaultAsync(u => u.Id == unitId && u.OrgId == orgId);
            if (unit == null)
                return NotFound("Unit not found");
            var key = new UnitKey
            {
                OrgId = orgId,
                UnitId = unitId,
                Label = request.Label,
                Code = request.Code,
                IssuedAt = request.IssuedAt,
                HolderUserId = request.HolderUserId
            };
            _db.UnitKeys.Add(key);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetKeys), new { unitId = unitId }, key);
        }

        // PUT: /api/keys/{id}
        [HttpPut("api/keys/{id}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> UpdateKey([FromRoute] Guid id, [FromBody] UpdateUnitKeyRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var key = await _db.UnitKeys.FirstOrDefaultAsync(k => k.Id == id && k.OrgId == orgId);
            if (key == null)
                return NotFound();
            if (request.Label != null) key.Label = request.Label;
            if (request.Code != null) key.Code = request.Code;
            if (request.IssuedAt != null) key.IssuedAt = request.IssuedAt;
            if (request.HolderUserId != null) key.HolderUserId = request.HolderUserId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/keys/{id}
        [HttpDelete("api/keys/{id}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> DeleteKey([FromRoute] Guid id)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var key = await _db.UnitKeys.FirstOrDefaultAsync(k => k.Id == id && k.OrgId == orgId);
            if (key == null)
                return NotFound();
            _db.UnitKeys.Remove(key);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}