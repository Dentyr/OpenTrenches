using Godot;
using OpenTrenches.Common.Resources;

namespace OpenTrenches.Common.Ability;

public record class AbilityRecord(int ID)
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required float Cooldown { get; init; }
    public float Duration{ get; init; } = 0;
    public int Cost { get; init; } = 1;

    public float DefenseMod{ get; init; } = 0;
}
