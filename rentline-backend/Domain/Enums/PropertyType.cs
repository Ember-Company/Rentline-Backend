namespace rentline_backend.Domain.Enums
{
    /// <summary>
    ///     Describes the high‑level classification of a property. Landlords
    ///     may manage a variety of property types ranging from single‑family
    ///     homes to commercial buildings. New values should be appended to
    ///     maintain existing numeric mappings in the database.
    /// </summary>
    public enum PropertyType
    {
        Residential = 0,
        Commercial = 1,
        Industrial = 2,
        Land = 3,
        MixedUse = 4
    }
}