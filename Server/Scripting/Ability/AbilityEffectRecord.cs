using System;
using OpenTrenches.Common.Ability;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scripting.Ability;

/// <summary>
/// The associated server-side effects with a common ability record
/// </summary>
public record class AbilityEffectRecord
{
    private static readonly Func<Character, bool> DefaultCanUse = (c) => true;
    private static readonly Action<Character, ICharacterAdapter> DefaultFinishEffect = (c, a) => {};

    public readonly Func<Character, bool> CanUse;
    public readonly Action<Character, ICharacterAdapter> FinishEffect;

    public AbilityEffectRecord(
        Func<Character, bool>? CanUse = null,
        Action<Character, ICharacterAdapter>? FinishEffect = null
    )
    {
        this.CanUse = CanUse ?? DefaultCanUse;
        this.FinishEffect = FinishEffect ?? DefaultFinishEffect;
    }
}