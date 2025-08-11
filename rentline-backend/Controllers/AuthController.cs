using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rentline_backend.DTOs;
using rentline_backend.Services;
using rentline_backend.Domain.Entities;
using rentline_backend.Domain.Enums;
using rentline_backend.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rentline_backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly RentlineDbContext _db;
        private readonly IJwtTokenService _tokenService;
        private readonly IPasswordHasher _hasher;

        public AuthController(RentlineDbContext db, IJwtTokenService tokenService, IPasswordHasher hasher)
        {
            _db = db;
            _tokenService = tokenService;
            _hasher = hasher;
        }

        /// <summary>
        ///     Registers a new organisation and its first user. Returns a
        ///     JWT token for subsequent authenticated requests.
        /// </summary>
        [HttpPost("register-org")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterOrg([FromBody] RegisterOrgRequest request)
        {
            // Validate unique email
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return Conflict("Email already in use.");
            }

            var org = new Organization
            {
                Name = request.OrgName,
                OrgType = request.OrgType
            };

            var user = new AppUser
            {
                OrgId = org.Id,
                Email = request.Email,
                PasswordHash = _hasher.HashPassword(request.Password),
                DisplayName = request.DisplayName,
                Role = request.OrgType == OrgType.Landlord ? Role.Landlord : Role.AgencyAdmin,
                IsEmailVerified = false
            };

            org.Users.Add(user);
            _db.Organizations.Add(org);
            await _db.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token, orgId = org.Id, role = user.Role.ToString() });
        }

        /// <summary>
        ///     Authenticates a user with email and password. Returns a JWT
        ///     on success.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            if (!_hasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token, orgId = user.OrgId, role = user.Role.ToString() });
        }

        /// <summary>
        ///     Accepts an invitation using the provided token, creating a
        ///     new user under the organisation that issued the invite.
        /// </summary>
        [HttpPost("accept-invite")]
        [AllowAnonymous]
        public async Task<IActionResult> AcceptInvite([FromBody] AcceptInviteRequest request)
        {
            var invite = await _db.Invites.FirstOrDefaultAsync(i => i.Token == request.Token);
            if (invite == null || invite.Used || invite.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest("Invalid or expired invite.");
            }

            // Ensure the email is still available
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == invite.Email);
            if (existingUser != null)
            {
                return Conflict("Email already in use.");
            }

            var user = new AppUser
            {
                OrgId = invite.OrgId,
                Email = invite.Email,
                PasswordHash = _hasher.HashPassword(request.Password),
                DisplayName = request.DisplayName,
                Role = invite.Role,
                IsEmailVerified = true
            };

            invite.Used = true;
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token, orgId = user.OrgId, role = user.Role.ToString() });
        }
    }
}