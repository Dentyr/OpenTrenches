using OpenTrenches.Server.Scripting.Player;

namespace OpenTrenches.Server.Scripting.World;

/// <summary>
/// Backend facing interface for querying against the world
/// </summary>
public interface IWorld2DQueryService
{
    WorldQueryResult Query(QueryContext context, WorldQuery query);
}
