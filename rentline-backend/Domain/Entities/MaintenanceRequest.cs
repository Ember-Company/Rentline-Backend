using System;
using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Common;
using rentline_backend.Domain.Enums;

namespace rentline_backend.Domain.Entities
{
    /// <summary>
    ///     Represents a request to repair or maintain a unit. Tenants
    ///     create maintenance requests, and landlords or designated
    ///     maintenance staff manage and update them.
    /// </summary>
    public class MaintenanceRequest : OrgEntity
    {
        [Required]
        public Guid UnitId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = default!;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = default!;

        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Pending;

        /// <summary>
        ///     Identifier of the user who created this request.
        /// </summary>
        [Required]
        public Guid CreatedByUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Unit? Unit { get; set; }
        public AppUser? CreatedByUser { get; set; }
    }
}