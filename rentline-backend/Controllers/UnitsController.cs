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
    [Route("api/units")]
    [Authorize(Policy = "OrgMember")]
    public class UnitsController : ControllerBase
    {
        private readonly RentlineDbContext _db;
        public UnitsController(RentlineDbContext db)
        {
            _db = db;
        }

        // GET: /api/units/by-property/{propertyId}
        [HttpGet("by-property/{propertyId}")]
        public async Task<IActionResult> GetUnitsByProperty([FromRoute] Guid propertyId)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var units = await _db.Units
                .Where(u => u.PropertyId == propertyId && u.OrgId == orgId)
                .ToListAsync();
            return Ok(units);
        }

        // POST: /api/units
        [HttpPost]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> CreateUnit([FromBody] CreateUnitRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            // Validate property ownership
            var property = await _db.Properties.FirstOrDefaultAsync(p => p.Id == request.PropertyId && p.OrgId == orgId);
            if (property == null)
                return NotFound("Property not found");
            var unit = new Unit
            {
                OrgId = orgId,
                PropertyId = request.PropertyId,
                UnitNumber = request.UnitNumber,
                Bedrooms = request.Bedrooms,
                Bathrooms = request.Bathrooms,
                AreaSqm = request.AreaSqm,
                RentAmount = request.RentAmount,
                Currency = request.Currency,
                UnitType = request.UnitType,
                Status = request.Status,
                TenantId = request.TenantUserId,
                MarketRent = request.MarketRent,
                DepositAmount = request.DepositAmount,
                DepositCurrency = request.DepositCurrency,
                Notes = request.Notes
            };
            _db.Units.Add(unit);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUnitsByProperty), new { propertyId = request.PropertyId }, unit);
        }

        // PUT: /api/units/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> UpdateUnit([FromRoute] Guid id, [FromBody] UpdateUnitRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var unit = await _db.Units.FirstOrDefaultAsync(u => u.Id == id && u.OrgId == orgId);
            if (unit == null)
                return NotFound();
            if (request.UnitNumber != null) unit.UnitNumber = request.UnitNumber;
            if (request.Bedrooms != null) unit.Bedrooms = request.Bedrooms;
            if (request.Bathrooms != null) unit.Bathrooms = request.Bathrooms;
            if (request.AreaSqm != null) unit.AreaSqm = request.AreaSqm;
            if (request.RentAmount != null) unit.RentAmount = request.RentAmount;
            if (request.Currency != null) unit.Currency = request.Currency;
            if (request.UnitType != null) unit.UnitType = request.UnitType.Value;
            if (request.Status != null) unit.Status = request.Status.Value;
            if (request.TenantUserId != null) unit.TenantId = request.TenantUserId;
            if (request.MarketRent != null) unit.MarketRent = request.MarketRent;
            if (request.DepositAmount != null) unit.DepositAmount = request.DepositAmount;
            if (request.DepositCurrency != null) unit.DepositCurrency = request.DepositCurrency;
            if (request.Notes != null) unit.Notes = request.Notes;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/units/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> DeleteUnit([FromRoute] Guid id)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var unit = await _db.Units.FirstOrDefaultAsync(u => u.Id == id && u.OrgId == orgId);
            if (unit == null)
                return NotFound();
            _db.Units.Remove(unit);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}