using Godot;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Server.Scene.World;

namespace OpenTrenches.Server.Scripting.Player;

public interface ICharacterAdapter
{
    IWorldSimulator World { get; } 
    

    /// <summary>
    /// Simulates this character shooting at <paramref name="target"/>
    /// </summary>
    /// <returns>Character, if it hits one</returns>
    public FireHitResult AdaptFire(Vector3 target);
}