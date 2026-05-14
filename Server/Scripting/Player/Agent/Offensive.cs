using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;


public class Offensive : AbstractObjective
{
    private const float GatheringRadius = 6f;
    /// <remarks>
    /// math helper
    /// </remarks>
    private const float GatheringRadiusSquared = GatheringRadius * GatheringRadius;

    public Vector2 Gathering { get; private set; }
    public Vector2 Target { get; private set; }

    private List<CharacterAgent> _assignedAgents = [];
    public IReadOnlyList<CharacterAgent> AssignedAgents => _assignedAgents;

    /// <summary>
    /// 
    /// </summary>
    private Phase _combatPhase = Phase.Gathering;

    public Offensive(Vector2 gathering, Vector2 target)
    {
        Gathering = gathering;
        Target = target;
    }

    public void Assign(CharacterAgent agent)
    {
        _assignedAgents.Add(agent);

        switch (_combatPhase)
        {
            case Phase.Gathering:
                agent.AssignTask(new PositionTask(Gathering));
                break;
            case Phase.Assaulting:
                agent.AssignTask(new PositionTask(Gathering));
                break;
            case Phase.Consolidating:
                break;
        }
    }
    public void Unassign(CharacterAgent agent)
    {
        _assignedAgents.Remove(agent);
    }

    private void StartAssault(IWorld2DQueryService service, IServerChunkArray chunkArray)
    {
        _combatPhase = Phase.Assaulting;
        foreach (CharacterAgent agent in _assignedAgents)
        {
            agent.AssignTask(new PositionTask(Target));
        }
    }

    /// <summary>
    /// Returns to the gathering location to hold that position 
    /// </summary>
    private void Regroup(IWorld2DQueryService service, IServerChunkArray chunkArray)
    {
        
    }

    /// <summary>
    /// Treats the gathering point as lost. Attempts to 
    /// </summary>
    /// <param name="service"></param>
    /// <param name="chunkArray"></param>
    private void Rout(IWorld2DQueryService service, IServerChunkArray chunkArray)
    {
        
    }

    public override void Strategize(IWorld2DQueryService service, IServerChunkArray chunkArray)
    {
        switch (_combatPhase)
        {
            case Phase.Gathering:
                IEnumerable<CharacterAgent> farAgents = _assignedAgents.Where(agent => agent.Character.Position.DistanceSquaredTo(Target) < GatheringRadiusSquared);
                // If all agents are close enough and no longer repositioning, procede to next phase. Any agent not moving to location is set to move to location
                if (!farAgents.Any())
                {
                    StartAssault(service, chunkArray);
                }
                else
                {
                    foreach (CharacterAgent unassinged in farAgents.Where(agent => agent.Task is not PositionTask task))
                    {
                        unassinged.AssignTask(new PositionTask(Gathering));
                    }
                }
                break;
            case Phase.Assaulting:

                break;
            case Phase.Consolidating:
                break;
        }
    }

    public enum Phase
    {
        /// <summary>
        /// Gather all units at a semi-safe frontline location
        /// </summary>
        Gathering,
        /// <summary>
        /// Moved all units to the target point
        /// </summary>
        Assaulting,
        /// <summary>
        /// Defend the forward point to establish new offensive point
        /// </summary>
        Consolidating,
        /// <summary>
        /// When the gathering point is lost, units will hold their ground until more orders are received
        /// </summary>
        Routed,
    }
}