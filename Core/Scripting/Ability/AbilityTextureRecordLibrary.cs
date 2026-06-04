

using System;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Ability;
using OpenTrenches.Common.Resources;

namespace OpenTrenches.Core.Scripting.Ability;


/// <summary>
/// A library of server-side effects
/// </summary>
public static class AbilityTextureRecordLibrary
{
    public static readonly IReadOnlyDictionary<AbilityRecord, AbilityTextureRecord> Textures = new Dictionary<AbilityRecord, AbilityTextureRecord>() {
        {
            AbilityRecords.StimulantAbility, 
            new(
                Thumbnail: TextureLibrary2D.Ability.Stimulant
            )
        },
        {
            AbilityRecords.AirstrikeAbility, 
            new(
                Thumbnail: TextureLibrary2D.Ability.Airstrike
            )
        }
    };
}