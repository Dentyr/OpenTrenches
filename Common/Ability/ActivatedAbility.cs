
using System;

namespace OpenTrenches.Common.Ability;

public class ActivatedAbility(AbilityRecord Record) : IActivatedAbility
{
    public AbilityRecord Record { get; } = Record;
    
    /// <summary>
    /// Time until the ability is available again
    /// </summary>
    public float TimeLeft { get; private set; } = 0;

    /// <summary>
    /// Time until the ability stoped being active.
    /// </summary>
    public float Duration { get; private set; } = 0;

    public bool Active => Duration > 0;

    /// <summary>
    /// Advanced <see cref="TimeLeft"> and <see cref="Duration"/> by <paramref name="time"/>, until they reach 0.
    /// </summary>
    /// <param name="time"></param>
    public void ProgressTimer(float time)
    {
        TimeLeft = Math.Max(0, TimeLeft - time);
        Duration = Math.Max(0, Duration - time);
    }

    /// <summary>
    /// Acts as if the ability has been activated, setting timer to cooldown and duration to ability duration.
    /// </summary>
    public void ActivateTimer()
    {
        TimeLeft = Record.Cooldown;
        Duration = Record.Duration;
    }
}
