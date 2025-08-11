using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rentline_backend.Domain.Entities;
using rentline_backend.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rentline_backend.Controllers
{
    [ApiController]
    [Route("api/org")]
    [Authorize(Policy = "OrgMember")]
    public class OrganizationController : ControllerBase
    {
        private readonly RentlineDbContext _db;
        public OrganizationController(RentlineDbContext db)
        {
            _db = db;
        }

        /// <summary>
        ///     Returns information about the current organisation, including
        ///     its users, properties and units.
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetOrganization()
        {
            var orgIdClaim = User.FindFirst("orgId")?.Value;
            if (orgIdClaim == null)
                return Forbid();

            var orgId = Guid.Parse(orgIdClaim);
            var org = await _db.Organizations
                .Include(o => o.Users)
                .Include(o => o.Properties)
                    .ThenInclude(p => p.Units)
                .FirstOrDefaultAsync(o => o.Id == orgId);

            if (org == null)
                return NotFound();

            return Ok(new
            {
                org.Id,
                org.Name,
                org.OrgType,
                Users = org.Users.Select(u => new { u.Id, u.Email, u.DisplayName, u.Role }),
                Properties = org.Properties.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Street,
                    p.City,
                    p.State,
                    p.PostalCode,
                    p.Country,
                    p.Description,
                    p.Type,
                    p.PurchaseDate,
                    p.PurchasePrice,
                    p.YearBuilt,
                    p.LotSizeSqm,
                    Units = p.Units.Select(u => new
                    {
                        u.Id,
                        u.UnitNumber,
                        u.Bedrooms,
                        u.Bathrooms,
                        u.AreaSqm,
                        u.RentAmount,
                        u.Currency,
                        u.UnitType,
                        u.Status,
                        u.MarketRent,
                        u.DepositAmount,
                        u.DepositCurrency,
                        u.TenantId,
                        u.Notes
                    })
                })
            });
        }
    }
}