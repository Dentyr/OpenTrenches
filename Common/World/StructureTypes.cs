using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OpenTrenches.Common.World;

public static class StructureTypes
{
    private static readonly Dictionary<StructureEnum, StructureType> _all = new()
    {
        { 
            StructureEnum.Camp, 
            new(StructureEnum.Camp, new(-1, -1, 3, 3))
            {
                Constructable = false,
                Hp = 100,
                Persistent = true,
            }
        }
    };


    public static IReadOnlyDictionary<StructureEnum, StructureType> All { get; } = new ReadOnlyDictionary<StructureEnum, StructureType>(_all);


    public static bool TryGet<T>(T? type, out StructureType? equipment) where T : struct, Enum
    {
        equipment = null;
        if (type is null) return false;
        if (type is StructureEnum firearmEnum) return TryGet(firearmEnum, out equipment);
        return false;
    }
    public static bool TryGet(StructureEnum? type, out StructureType? equipment)
    {
        if (type is not StructureEnum notnull)
        {
            equipment = null;
            return false;
        }
        equipment = Get(notnull);
        return true;
    }
    public static StructureType Get(StructureEnum type) =>
        _all.TryGetValue(type, out var equipment) ? equipment : throw new ArgumentException($"unregistered structure: {type}");

    public static StructureType BaseCamp => Get(StructureEnum.Camp);
}