using System.Linq;
using OpenTrenches.Common.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

public static class TaskServices
{
    /// <summary>
    /// Searches nearby units for the nearest enemy structure or character
    /// </summary>
    public static IWorldObject? FindTarget(Character character, ICharacterAdapter adapter)
    {
        WorldQueryResult possible = adapter.Query(new WorldQuery.Threats());
        if (possible is WorldQueryResult.Found found)
        {
            if (found.Characters.Count > 0 &&
                found.Characters.MinBy(target => target.Position.DistanceSquaredTo(character.Position)) is IWorldObject target)
            {
                return target;
            }
        }
        return null;
    }
}
