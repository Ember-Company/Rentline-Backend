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
    [Route("api/properties")]
    [Authorize(Policy = "OrgMember")]
    public class PropertiesController : ControllerBase
    {
        private readonly RentlineDbContext _db;
        public PropertiesController(RentlineDbContext db)
        {
            _db = db;
        }

        // GET: /api/properties
        [HttpGet]
        public async Task<IActionResult> GetProperties()
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var properties = await _db.Properties
                .Where(p => p.OrgId == orgId)
                .Include(p => p.Units)
                .ToListAsync();
            return Ok(properties);
        }

        // POST: /api/properties
        [HttpPost]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var property = new Property
            {
                OrgId = orgId,
                Name = request.Name,
                Street = request.Street,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                Description = request.Description,
                Type = request.Type,
                PurchaseDate = request.PurchaseDate,
                PurchasePrice = request.PurchasePrice,
                YearBuilt = request.YearBuilt,
                LotSizeSqm = request.LotSizeSqm
            };
            _db.Properties.Add(property);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProperties), new { id = property.Id }, property);
        }

        // PUT: /api/properties/{id}
        [HttpPut("{id}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> UpdateProperty([FromRoute] Guid id, [FromBody] UpdatePropertyRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var property = await _db.Properties.FirstOrDefaultAsync(p => p.Id == id && p.OrgId == orgId);
            if (property == null)
                return NotFound();

            // Update only provided fields
            if (request.Name != null) property.Name = request.Name;
            if (request.Street != null) property.Street = request.Street;
            if (request.City != null) property.City = request.City;
            if (request.State != null) property.State = request.State;
            if (request.PostalCode != null) property.PostalCode = request.PostalCode;
            if (request.Country != null) property.Country = request.Country;
            if (request.Description != null) property.Description = request.Description;
            if (request.Type != null) property.Type = request.Type.Value;
            if (request.PurchaseDate != null) property.PurchaseDate = request.PurchaseDate;
            if (request.PurchasePrice != null) property.PurchasePrice = request.PurchasePrice;
            if (request.YearBuilt != null) property.YearBuilt = request.YearBuilt;
            if (request.LotSizeSqm != null) property.LotSizeSqm = request.LotSizeSqm;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: /api/properties/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "OwnerOrManager")]
        public async Task<IActionResult> DeleteProperty([FromRoute] Guid id)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var property = await _db.Properties.FirstOrDefaultAsync(p => p.Id == id && p.OrgId == orgId);
            if (property == null)
                return NotFound();
            _db.Properties.Remove(property);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // POST: /api/properties/{id}/images
        [HttpPost("{id}/images")]
        [Authorize(Policy = "OwnerOrManager")]
        public IActionResult UploadImage([FromRoute] Guid id)
        {
            // This endpoint would typically accept multipart/form-data to
            // upload an image to a cloud provider. As a placeholder, we
            // return NotImplemented. Implement integration with a
            // storage service such as Cloudinary or AWS S3 here.
            return StatusCode(501, "Image upload not implemented in this template.");
        }
    }
}