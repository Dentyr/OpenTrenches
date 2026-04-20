using Godot;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Server.Scene.World;

namespace OpenTrenches.Server.Scripting.Player;

public interface ICharacterAdapter
{
    /// <summary>
    /// Simulates this character shooting at <paramref name="target"/>
    /// </summary>
    /// <returns>Character, if it hits one</returns>
    public FireHitResult AdaptFire(Vector2 target);
    /// <summary>
    /// Tries to move out of trench by going to the nearest ground tile in <paramref name="direction"/>, if it's in range
    /// </summary>
    /// <returns>null if failed to jump, the position if successful and unoccupied</returns>
    Vector2? AdaptJump(Vector2 direction);


    WorldQueryResult Query(WorldQuery query);
}
