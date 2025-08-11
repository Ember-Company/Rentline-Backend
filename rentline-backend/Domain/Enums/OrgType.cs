namespace rentline_backend.Domain.Enums
{
    /// <summary>
    ///     Represents the type of organisation using the system. A landlord
    ///     operates their own portfolio whereas an agency manages
    ///     properties on behalf of multiple landlords.
    /// </summary>
    public enum OrgType
    {
        Landlord = 0,
        Agency = 1
    }
}
