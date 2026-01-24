using Godot;

namespace OpenTrenches.Server.Scripting.Player;

public interface ICharacterAdapter
{
    /// <summary>
    /// Simulates this character shooting at <paramref name="target"/>
    /// </summary>
    /// <returns>Character, if it hits one</returns>
    public Character? AdaptFire(Vector3 target);
}