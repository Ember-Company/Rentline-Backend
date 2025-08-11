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
    public class UnitEquipmentController : ControllerBase
    {
        private readonly RentlineDbContext _db;
        public UnitEquipmentController(RentlineDbContext db)
        {
            _db = db;
        }

        // GET: /api/units/{unitId}/equipment
        [HttpGet("api/units/{unitId}/equipment")]
        public async Task<IActionResult> GetEquipment([FromRoute] Guid unitId)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var items = await _db.UnitEquipment
                .Where(e => e.UnitId == unitId && e.OrgId == orgId)
                .ToListAsync();
            return Ok(items);
        }

        // POST: /api/units/{unitId}/equipment
        [HttpPost("api/units/{unitId}/equipment")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> CreateEquipment([FromRoute] Guid unitId, [FromBody] CreateUnitEquipmentRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            if (request.UnitId != Guid.Empty && request.UnitId != unitId)
                return BadRequest("UnitId mismatch between URL and body.");
            var unit = await _db.Units.FirstOrDefaultAsync(u => u.Id == unitId && u.OrgId == orgId);
            if (unit == null)
                return NotFound("Unit not found");
            var equipment = new UnitEquipment
            {
                OrgId = orgId,
                UnitId = unitId,
                Name = request.Name,
                Category = request.Category,
                Brand = request.Brand,
                Model = request.Model,
                SerialNumber = request.SerialNumber,
                PurchaseDate = request.PurchaseDate,
                WarrantyExpiryDate = request.WarrantyExpiryDate,
                Condition = request.Condition,
                Notes = request.Notes
            };
            _db.UnitEquipment.Add(equipment);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEquipment), new { unitId = unitId }, equipment);
        }

        // PUT: /api/equipment/{id}
        [HttpPut("api/equipment/{id}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> UpdateEquipment([FromRoute] Guid id, [FromBody] UpdateUnitEquipmentRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var equipment = await _db.UnitEquipment.FirstOrDefaultAsync(e => e.Id == id && e.OrgId == orgId);
            if (equipment == null)
                return NotFound();
            if (request.Name != null) equipment.Name = request.Name;
            if (request.Category != null) equipment.Category = request.Category;
            if (request.Brand != null) equipment.Brand = request.Brand;
            if (request.Model != null) equipment.Model = request.Model;
            if (request.SerialNumber != null) equipment.SerialNumber = request.SerialNumber;
            if (request.PurchaseDate != null) equipment.PurchaseDate = request.PurchaseDate;
            if (request.WarrantyExpiryDate != null) equipment.WarrantyExpiryDate = request.WarrantyExpiryDate;
            if (request.Condition != null) equipment.Condition = request.Condition.Value;
            if (request.Notes != null) equipment.Notes = request.Notes;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/equipment/{id}
        [HttpDelete("api/equipment/{id}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> DeleteEquipment([FromRoute] Guid id)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var equipment = await _db.UnitEquipment.FirstOrDefaultAsync(e => e.Id == id && e.OrgId == orgId);
            if (equipment == null)
                return NotFound();
            _db.UnitEquipment.Remove(equipment);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}