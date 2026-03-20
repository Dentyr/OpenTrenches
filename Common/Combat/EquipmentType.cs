namespace OpenTrenches.Common.Combat;

public class FirearmType : AbstractEquipmentType<FirearmEnum>
{
    public FirearmStats Stats { get; }
    public required string Name { get; init; }
    public required string Description { get; init; }

    public FirearmType(FirearmEnum id, int logisticsCost, FirearmStats stats)
        : base(id, EquipmentCategory.Firearm, logisticsCost)
    {
        Stats = stats;
    }
}
