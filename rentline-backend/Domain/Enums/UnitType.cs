namespace rentline_backend.Domain.Enums
{
    /// <summary>
    ///     Describes the configuration of a unit. This enum should be kept in
    ///     sync with supported unit types in the frontend. New values must be
    ///     appended to avoid altering existing integer mappings stored in the
    ///     database.
    /// </summary>
    public enum UnitType
    {
        Studio = 0,
        OneBedroom = 1,
        TwoBedroom = 2,
        ThreeBedroom = 3,
        Penthouse = 4,
        Duplex = 5,
        Storage = 6
    }
}