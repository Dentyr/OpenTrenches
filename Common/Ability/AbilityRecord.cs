using Godot;
using OpenTrenches.Common.Resources;

namespace OpenTrenches.Common.Ability;

public record class AbilityRecord
{
    public required int ID { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public float DefenseMod{ get; init; } = 0;
    public required float Cooldown { get; init; }
    public float Duration{ get; init; } = 0;

    public Texture2D Thumbnail = TextureLibrary2D.NotFound;
    
}
