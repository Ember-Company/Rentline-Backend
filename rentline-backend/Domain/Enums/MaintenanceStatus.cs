namespace rentline_backend.Domain.Enums
{
    /// <summary>
    ///     Represents the lifecycle state of a maintenance request.
    /// </summary>
    public enum MaintenanceStatus
    {
        Pending = 0,
        InProgress = 1,
        Resolved = 2,
        Closed = 3
    }
}