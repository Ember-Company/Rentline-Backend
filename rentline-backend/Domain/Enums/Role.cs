namespace rentline_backend.Domain.Enums
{
    /// <summary>
    ///     Roles determine what actions a user can perform in the system.
    ///     Values align with policies defined in the API. Do not reorder
    ///     these values to avoid breaking persisted role assignments.
    /// </summary>
    public enum Role
    {
        Landlord = 0,
        AgencyAdmin = 1,
        Manager = 2,
        Maintenance = 3,
        Tenant = 4,
        Viewer = 5
    }
}