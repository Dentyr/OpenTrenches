using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;



/// <summary>
/// Objectives are goals a team's AI wants to fulfil
/// </summary>
public abstract class AbstractObjective
{
    public abstract void Strategize(IWorld2DQueryService service, IServerChunkArray chunkArray);
}