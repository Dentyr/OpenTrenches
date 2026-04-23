using System.Linq;
using OpenTrenches.Common.World;

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
    public static IWorldObject? FindTarget(Character character, ICharacterAdapter adapter)
    {
        WorldQueryResult possible = adapter.Query(_threatQuery);
        if (possible.Characters.Count > 0 &&
            possible.Characters.MinBy(target => target.Position.DistanceSquaredTo(character.Position)) is IWorldObject target)
        {
            return target;
        }
        return null;
    }
}
