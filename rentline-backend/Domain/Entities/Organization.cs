using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Common;
using rentline_backend.Domain.Enums;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     Represents a landlord or agency organisation. Organisations own
    ///     portfolios of properties and have multiple users with various
    ///     roles.
    /// </summary>
    public class Organization : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        /// <summary>
        ///     Indicates whether this organisation is a single landlord
        ///     (managing their own properties) or an agency that manages
        ///     properties for multiple landlords.
        /// </summary>
        public OrgType OrgType { get; set; } = OrgType.Landlord;

        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}