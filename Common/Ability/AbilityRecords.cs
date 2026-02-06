namespace OpenTrenches.Common.Ability;

public static class AbilityRecords
{
    public static AbilityRecord StimulantAbility { get; } = new(0)
    {
        Name = "Stims",
        Description = "gain defense",
        DefenseMod = 5,
        Cooldown = 60f,
        Duration = 15f,
    };
}
