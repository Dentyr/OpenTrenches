using Godot;
using OpenTrenches.Common.Resources;

namespace OpenTrenches.Common.Ability;

public record class AbilityRecord
{
    public required int ID;
    public required string Name;
    public required string Description;
    public float DefenseMod = 0;
    public required float Cooldown;
    public float Duration = 0;

    public Texture2D Thumbnail = TextureLibrary2D.NotFound;
    
}
