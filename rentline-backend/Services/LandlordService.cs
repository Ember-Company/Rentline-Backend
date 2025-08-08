using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using rentline_backend.Data;
using rentline_backend.Domain;
using rentline_backend.DTOs;

namespace rentline_backend.Services;

public interface ILandlordService
{
    Task<IEnumerable<LandlordDto>> GetLandlordsAsync(Guid orgId);
}

public class LandlordService(AppDbContext db) : ILandlordService
{
    public async Task<IEnumerable<LandlordDto>> GetLandlordsAsync(Guid orgId)
    {
        return await db.Users.Where(u => u.OrganizationId == orgId && u.Role == UserRole.Landlord)
            .Select(u => new LandlordDto(u.Id, u.Email, u.DisplayName))
            .ToListAsync();

    }
}
