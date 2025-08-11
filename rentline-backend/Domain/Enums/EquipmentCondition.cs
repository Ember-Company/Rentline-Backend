namespace rentline_backend.Domain.Enums
{
    /// <summary>
    ///     Represents the physical state of a piece of equipment. Use this
    ///     enumeration to quickly assess maintenance needs.
    /// </summary>
    public enum EquipmentCondition
    {
        New = 0,
        Good = 1,
        Fair = 2,
        NeedsRepair = 3,
        Broken = 4
    }
}