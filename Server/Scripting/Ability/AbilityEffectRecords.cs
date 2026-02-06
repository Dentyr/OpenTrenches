using System;
using System.Collections.Generic;
using OpenTrenches.Common.Ability;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scripting.Ability;

/// <summary>
/// A library of server-side effects
/// </summary>
public static class AbilityEffectRecords
{
    static AbilityEffectRecords()
    {
        var tempDict = new Dictionary<int, AbilityEffectRecord>();
        Append(tempDict, 
            AbilityRecords.StimulantAbility, 
            canDo: (chara) => true
        );
        Effects = tempDict;
    }

    private static void Append(Dictionary<int, AbilityEffectRecord> dict, AbilityRecord abilityRecord, Func<Character, bool> canDo)
    {
        if (!dict.TryAdd(abilityRecord.ID, new(
            Info: abilityRecord,
            CanUse: canDo
        ))) throw new Exception("Failed to add ability to dictionary");
    }

    public static IReadOnlyDictionary<int, AbilityEffectRecord> Effects { get; }
}