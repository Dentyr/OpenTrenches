using Godot;
using OpenTrenches.Common.Contracts.DTO;

namespace OpenTrenches.Server.Scripting.Player;

public interface ICharacterAdapter
{
    void AdaptBuild(Vector2I cell, TileType buildTarget, float progress);

    /// <summary>
    /// Simulates this character shooting at <paramref name="target"/>
    /// </summary>
    /// <returns>Character, if it hits one</returns>
    public Character? AdaptFire(Vector3 target);
}