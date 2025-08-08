using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using rentline_backend.Data;
using rentline_backend.Domain;
using rentline_backend.DTOs;

namespace rentline_backend.Services;

public class PropertyService(AppDbContext db)
{
    public async Task<PropertyDto> CreatePropertyAsync(
    CreatePropertyRequest request,
    Guid currentUserId,
    UserRole currentUserRole,
    Organization org)
    {
        Guid? ownerId;


        if (org.Type == OrgType.Landlord ||
            (org.Type == OrgType.Agency && currentUserRole == UserRole.Landlord))
        {
            ownerId = currentUserId;
        }
        else if (org.Type == OrgType.Agency)
        {
            if (currentUserRole != UserRole.AgencyAdmin && currentUserRole != UserRole.Manager)
                throw new UnauthorizedAccessException();

            if (request.OwnerUserId == null)
                throw new ArgumentException("OwnerUserId is required when agency admins/managers create a property.");

            var owner = await db.Users.FirstOrDefaultAsync(u =>
                    u.Id == request.OwnerUserId &&
                    u.OrganizationId == org.Id &&
                    u.Role == UserRole.Landlord);

            if (owner == null)
                throw new ArgumentException("Specified owner is not a landlord in this organisation.");

            ownerId = request.OwnerUserId;
        }
        else
        {
            throw new InvalidOperationException("Unsupported organisation type for property creation.");
        }

        var property = new Property
        {
            Id = Guid.NewGuid(),
            OrganizationId = org.Id,
            OwnerUserId = ownerId,
            Name = request.Name,
            Street = request.Street,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country
        };

        db.Properties.Add(property);
        await db.SaveChangesAsync();

        return new PropertyDto
        {
            Id = property.Id,
            Name = property.Name,
            Street = property.Street,
            City = property.City,
            State = property.State,
            PostalCode = property.PostalCode,
            Country = property.Country,
            OwnerUserId = property.OwnerUserId
        };
    }

}
