using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rentline_backend.Domain.Enums;
using rentline_backend.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rentline_backend.Controllers
{
    [ApiController]
    [Route("api/tenants")]
    [Authorize(Policy = "OwnerOrManager")]
    public class TenantsController(RentlineDbContext db) : ControllerBase
    {

        // GET: /api/tenants
        [HttpGet]
        public async Task<IActionResult> GetTenants()
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            var tenants = await Task.Run(() => db.Users
                .Where(u => u.OrgId == orgId && u.Role == Role.Tenant)
                .Select(u => new { u.Id, u.Email, u.DisplayName })
                .ToList());
            return Ok(tenants);
        }
    }
}