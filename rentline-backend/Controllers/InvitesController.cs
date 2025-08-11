using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rentline_backend.DTOs;
using rentline_backend.Domain.Entities;
using rentline_backend.Data;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace rentline_backend.Controllers
{
    [ApiController]
    [Authorize(Policy = "OwnerOrManager")]
    [Route("api/invites")]
    public class InvitesController : ControllerBase
    {
        private readonly RentlineDbContext _db;
        public InvitesController(RentlineDbContext db)
        {
            _db = db;
        }

        // POST: /api/invites
        [HttpPost]
        public async Task<IActionResult> CreateInvite([FromBody] CreateInviteRequest request)
        {
            var orgId = Guid.Parse(User.FindFirst("orgId")!.Value);
            // Ensure email not already a user
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.OrgId == orgId);
            if (existingUser != null)
                return Conflict("A user with this email already exists.");
            // Generate a random token
            var tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            var token = Convert.ToBase64String(tokenBytes).TrimEnd('=');
            var invite = new Invite
            {
                OrgId = orgId,
                Email = request.Email,
                Token = token,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(request.ExpiresInDays),
                Used = false
            };
            _db.Invites.Add(invite);
            await _db.SaveChangesAsync();
            return Ok(new { invite.Token, invite.Email, invite.ExpiresAt });
        }
    }
}