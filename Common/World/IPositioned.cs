using Godot;

namespace OpenTrenches.Common.World;

/// <summary>
/// Any object that's positioned within worldspace
/// </summary>
public interface IWorldObject
{
    Vector2 Position { get; }

    float Hp { get; }
}

public record class WorldPosition(Vector2 Position) : IWorldObject
{
    public float Hp => 0;
}