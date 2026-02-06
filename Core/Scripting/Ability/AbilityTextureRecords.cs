

using System;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Ability;
using OpenTrenches.Common.Resources;

namespace OpenTrenches.Core.Scripting.Ability;


/// <summary>
/// A library of server-side effects
/// </summary>
public static class AbilityTextureRecords
{
    static AbilityTextureRecords()
    {
        var tempDict = new Dictionary<int, AbilityTextureRecord>();
        Append(tempDict, 
            AbilityRecords.StimulantAbility, 
            texture: TextureLibrary2D.StimulantThumbnail
        );
        Textures = tempDict;
    }

    private static void Append(Dictionary<int, AbilityTextureRecord> dict, AbilityRecord abilityRecord, Texture2D texture)
    {
        if (!dict.TryAdd(abilityRecord.ID, new(
            Info: abilityRecord,
            Thumbnail: texture
        ))) throw new Exception("Failed to add ability to dictionary");
    }

    public static IReadOnlyDictionary<int, AbilityTextureRecord> Textures { get; }
}