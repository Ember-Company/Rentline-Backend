using System;
using System.ComponentModel.DataAnnotations;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for creating a new maintenance request.
    /// </summary>
    public class CreateMaintenanceRequest
    {
        [Required]
        public Guid UnitId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = default!;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = default!;
    }
}