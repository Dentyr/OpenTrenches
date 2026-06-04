using System.Collections.Generic;

namespace OpenTrenches.Common.Ability;

public static class AbilityRecords
{
    public static AbilityRecord StimulantAbility { get; } = new(0)
    {
        Name = "Stims",
        Description = "gain defense",
        DefenseMod = 5,
        Cooldown = 60f,
        Duration = 10f,
        Cost = 5,
    };

    public static AbilityRecord AirstrikeAbility { get; } = new(0)
    {
        Name = "Airstrike",
        Description = "send a powerful blast at the view location in 3 seconds",
        Cooldown = 600f,
        Duration = 3f,
        Cost = 100,
    };

    /// <summary>
    /// Default abiliites charactesr have access too
    /// </summary>
    public static IReadOnlyCollection<AbilityRecord> DefaultAbilities = [StimulantAbility, AirstrikeAbility];
}
