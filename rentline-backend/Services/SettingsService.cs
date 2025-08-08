using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rentline_backend.Data;
using rentline_backend.Domain;
using rentline_backend.DTOs;

namespace rentline_backend.Services
{
    public interface ISettingsService
    {
        Task<UserSettingsDto> GetUserSettingsAsync(Guid userId, Guid orgId);
        Task<UserSettingsDto> UpdateUserSettingsAsync(Guid userId, Guid orgId, UpdateUserSettingsRequest request);
        Task<OrganizationSettingsDto> GetOrganizationSettingsAsync(Guid orgId);
        Task<OrganizationSettingsDto> UpdateOrganizationSettingsAsync(Guid orgId, UpdateOrganizationSettingsRequest request);
    }

    public class SettingsService : ISettingsService
    {
        private readonly AppDbContext _db;
        public SettingsService(AppDbContext db) => _db = db;

        public async Task<UserSettingsDto> GetUserSettingsAsync(Guid userId, Guid orgId)
        {
            var settings = await _db.UserSettings.FirstOrDefaultAsync(x => x.UserId == userId && x.OrganizationId == orgId);
            if (settings == null)
            {
                settings = new UserSetting { UserId = userId, OrganizationId = orgId };
                await _db.UserSettings.AddAsync(settings);
                await _db.SaveChangesAsync();
            }
            return new UserSettingsDto { Language = settings.Language, Theme = settings.Theme };
        }

        public async Task<UserSettingsDto> UpdateUserSettingsAsync(Guid userId, Guid orgId, UpdateUserSettingsRequest request)
        {
            var settings = await _db.UserSettings.FirstOrDefaultAsync(x => x.UserId == userId && x.OrganizationId == orgId);
            if (settings == null)
            {
                settings = new UserSetting { UserId = userId, OrganizationId = orgId };
                await _db.UserSettings.AddAsync(settings);
            }
            if (!string.IsNullOrWhiteSpace(request.Language)) settings.Language = request.Language!;
            if (!string.IsNullOrWhiteSpace(request.Theme)) settings.Theme = request.Theme!;
            await _db.SaveChangesAsync();
            return new UserSettingsDto { Language = settings.Language, Theme = settings.Theme };
        }

        public async Task<OrganizationSettingsDto> GetOrganizationSettingsAsync(Guid orgId)
        {
            var settings = await _db.OrganizationSettings.FirstOrDefaultAsync(x => x.OrganizationId == orgId);
            if (settings == null)
            {
                settings = new OrganizationSetting { OrganizationId = orgId };
                await _db.OrganizationSettings.AddAsync(settings);
                await _db.SaveChangesAsync();
            }
            return new OrganizationSettingsDto
            {
                DefaultLanguage = settings.DefaultLanguage,
                DefaultTheme = settings.DefaultTheme,
                Currency = settings.Currency
            };
        }

        public async Task<OrganizationSettingsDto> UpdateOrganizationSettingsAsync(Guid orgId, UpdateOrganizationSettingsRequest request)
        {
            var settings = await _db.OrganizationSettings.FirstOrDefaultAsync(x => x.OrganizationId == orgId);
            if (settings == null)
            {
                settings = new OrganizationSetting { OrganizationId = orgId };
                await _db.OrganizationSettings.AddAsync(settings);
            }
            if (!string.IsNullOrWhiteSpace(request.DefaultLanguage)) settings.DefaultLanguage = request.DefaultLanguage!;
            if (!string.IsNullOrWhiteSpace(request.DefaultTheme)) settings.DefaultTheme = request.DefaultTheme!;
            if (!string.IsNullOrWhiteSpace(request.Currency)) settings.Currency = request.Currency!;
            await _db.SaveChangesAsync();
            return new OrganizationSettingsDto
            {
                DefaultLanguage = settings.DefaultLanguage,
                DefaultTheme = settings.DefaultTheme,
                Currency = settings.Currency
            };
        }
    }
}
