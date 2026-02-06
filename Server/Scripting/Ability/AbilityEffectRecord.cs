using System;
using OpenTrenches.Common.Ability;
using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scripting.Ability;

/// <summary>
/// The associated server-side effects with a common ability record
/// </summary>
public record class AbilityEffectRecord
(
    AbilityRecord Info,
    Func<Character, bool> CanUse
) {}