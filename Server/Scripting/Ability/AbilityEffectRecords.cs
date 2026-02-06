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
    public readonly static IReadOnlyDictionary<AbilityRecord, AbilityEffectRecord> Effects  = new Dictionary<AbilityRecord, AbilityEffectRecord>() {
        {
            AbilityRecords.StimulantAbility,
            new (
                CanUse: (chara) => true
            )
        }
    };
}