using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// Responsible for generic agent methods.
/// </summary>
public static class TaskServices
{
    /// <summary>
    /// If the normalized absolute value of the x/y is greater than direction threashold, then they are considered to be moving in this direction.
    /// </summary>
    /// <remarks>If the character is moving with a velocity of (7f, 0.1f), this can be used to stop movement calculations from thinking the character's next cell with be close to (1, 1) </remarks>
    private const float DirectionThreshold = 0.2f;

    
    private static WorldQuery _threatQuery = new()
    {
        IncludeAlly = false,
        QueryArea = WorldQuery.Shape.Range
    };
    /// <summary>
    /// Searches nearby units for the nearest enemy structure or character
    /// </summary>
    public static IWorldObject? FindTarget(Character character, IWorld2DQueryService queryService)
    {
        WorldQueryResult possible = queryService.Query(QueryContext.MakeContext(character), _threatQuery);
        if (possible.Characters.Count > 0 &&
            possible.Characters.MinBy(target => target.Position.DistanceSquaredTo(character.Position)) is IWorldObject target)
        {
            return target;
        }
        return null;
    }



    /// <summary>
    /// Make <paramref name="character"/> fire if able, reloading if necessary
    /// </summary>
    public static void ReasonAttack(Character character)
    {
        // if character is in middle of reloading or has run out of ammo, return
        if (!character.PrimarySlot.Reloaded) return;
        if (character.PrimarySlot.AmmoStored == 0)
        {
            character.TryReload();
            return;
        }

        character.TrySet(Common.Contracts.Defines.CharacterState.Shooting);
    }

    /// <summary>
    /// Returns true if <paramref name="character"/> should continue to target <paramref name="enemy"/> within the range of <paramref name="maxDist"/>
    /// </summary>
    public static bool EnemyValid(Character character, [NotNullWhen(true)] IWorldObject? enemy, float maxDist)
    {
        return enemy is not null &&
            enemy.Hp > 0 &&
            enemy.Position.DistanceSquaredTo(character.Position) < (maxDist * maxDist);
    }

    /// <summary>
    /// Navigates <paramref name="character"/> to <paramref name="error"/> distance within <paramref name="position"/> using <paramref name="queryService"/> with a max algorithmic depth of <paramref name="maxDepth"/>
    /// </summary>
    /// <returns>True if destination has been reached</returns>
    public static bool Navigate(Character character, Vector2 position, IWorld2DQueryService queryService, int maxDepth = 5, float error = 1f)
    {
        if (character.Position.DistanceTo(position) <= error)
        {
            character.MoveIn(Vector2.Zero);
            return true;
        }
        character.MoveIn(position - character.Position);
        return false;
    }

    /// <summary>
    /// Reasons about the immediate next step for <paramref name="character"/> to move towards <paramref name="checkpoint"/>. Jumps out of trenches if necessary.
    /// Returns true if within <paramref name="error"/> of <paramref name="checkpoint"/>.
    /// </summary>
    /// <remarks>Meant to be used frequently, close to several times a second for best fluidity </remarks>
    public static bool Step(Character character, Vector2 checkpoint, IServerChunkArray chunkarray, float error = 1f)
    {
        if (character.Position.DistanceSquaredTo(checkpoint) < error * error)
            return true;
        switch (character.Layer)
        {
            case WorldLayer.Ground:
                break;
            case WorldLayer.Trench:
                Vector2 movementDirection = character.MovementVelocity.Normalized();
                Vector2I cellMovement = new(
                    x: Math.Abs(character.MovementVelocity.X) < DirectionThreshold ? 0 : Math.Sign(character.MovementVelocity.X),
                    y: Math.Abs(character.MovementVelocity.Y) < DirectionThreshold ? 0 : Math.Sign(character.MovementVelocity.Y)
                );
                if (chunkarray.TryGetTile(character.GetCell() + cellMovement, out var tile))
                {
                    if (tile == TileType.Clear)
                    {
                        character.TryExitTrench();
                    }
                }
                break;
        }
        return false;
    }
}
