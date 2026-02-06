namespace OpenTrenches.Common.Ability;

public interface IActivatedAbility
{
    public AbilityRecord Record { get; }
    
    /// <summary>
    /// Time until the ability is available again
    /// </summary>
    float TimeLeft { get; }
    /// <summary>
    /// Time until the ability stoped being active.
    /// </summary>
    float Duration { get; }

    bool Active { get; }
}