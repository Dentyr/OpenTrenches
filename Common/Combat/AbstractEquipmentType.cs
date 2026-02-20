using System;

namespace OpenTrenches.Common.Combat;

public abstract class AbstractEquipmentType
{
    public EquipmentEnum Id { get; }
    public EquipmentCategory Category { get; }
    public int LogisticsCost { get; }


    protected AbstractEquipmentType(EquipmentEnum id, EquipmentCategory category, int logisticsCost)
    {
        if (logisticsCost < 0) throw new ArgumentOutOfRangeException(nameof(logisticsCost));
        Id = id;
        Category = category;
        LogisticsCost = logisticsCost;
    }
}
