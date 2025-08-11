using System.ComponentModel.DataAnnotations;
using rentline_backend.Domain.Enums;

namespace rentline_backend.DTOs
{
    /// <summary>
    ///     Request payload for updating the status of a maintenance
    ///     request.
    /// </summary>
    public class UpdateMaintenanceStatusRequest
    {
        [Required]
        public MaintenanceStatus Status { get; set; }
    }
}