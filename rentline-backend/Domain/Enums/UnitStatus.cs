namespace rentline_backend.Domain.Enums
{
    /// <summary>
    ///     Indicates whether a unit is currently rented or vacant. Additional
    ///     statuses can be added in future (e.g. UnderMaintenance) without
    ///     breaking existing persisted values.
    /// </summary>
    public enum UnitStatus
    {
        Vacant = 0,
        Occupied = 1
    }
}