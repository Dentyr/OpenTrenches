namespace OpenTrenches.Common.Combat;

public class FirearmType : AbstractEquipmentType
{
    public FirearmStats Stats { get; }

    public FirearmType(FirearmEnum id, int logisticsCost, FirearmStats stats)
        : base(id, EquipmentCategory.Firearm, logisticsCost)
    {
        Stats = stats;
    }
}