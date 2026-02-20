using System;
using System.Collections.Generic;

namespace OpenTrenches.Common.Combat;

/// <typeparam name="T">Stat type</typeparam>
public class EquipmentType<T> : AbstractEquipmentType where T : class
{
    public T Stats { get; }

    public EquipmentType(EquipmentEnum id, EquipmentCategory category, int logisticsCost, T stats) : base(id, category, logisticsCost)
    {
        Stats = stats;
    }
}