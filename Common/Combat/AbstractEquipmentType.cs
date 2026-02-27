using System;

namespace OpenTrenches.Common.Combat;

public abstract class AbstractEquipmentType<T> where T : Enum
{
    public T Id { get; }
    public EquipmentCategory Category { get; }
    public int LogisticsCost { get; }


    protected AbstractEquipmentType(T id, EquipmentCategory category, int logisticsCost)
    {
        if (logisticsCost < 0) throw new ArgumentOutOfRangeException(nameof(logisticsCost));
        Id = id;
        Category = category;
        LogisticsCost = logisticsCost;
    }
}
