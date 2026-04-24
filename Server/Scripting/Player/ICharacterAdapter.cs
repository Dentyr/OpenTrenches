using Godot;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scene.World;

namespace OpenTrenches.Server.Scripting.Player;

public interface ICharacterAdapter
{
    Character Character { get; }
    

    /// <summary>
    /// Simulates this character shooting at <paramref name="target"/> with the firearm at <paramref name="channel"/> level.
    /// </summary>
    /// <returns>Character, if it hits one</returns>
    public FireHitResult AdaptFire(WorldLayer channel, Vector2 target);
    /// <summary>
    /// Tries to move out of trench by going to the nearest ground tile in <paramref name="direction"/>, if it's in range
    /// </summary>
    /// <returns>null if failed to jump, the position if successful and unoccupied</returns>
    Vector2? AdaptJump(Vector2 direction);


    WorldQueryResult Query(WorldQuery query);
}
