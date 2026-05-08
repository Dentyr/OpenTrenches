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
}
