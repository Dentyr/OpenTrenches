using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;


public class Offensive : AbstractObjective
{
    private const float GatheringRadius = 6f;
    private const float GatheringReadinesError = 3f;
    private const float GatheringReadinesRadius = GatheringRadius + GatheringReadinesError;
    /// <remarks>
    /// math helper
    /// </remarks>
    private const float GatheringReadinessSquared = GatheringReadinesRadius * GatheringReadinesRadius;

    /// <summary>
    /// % of troops needed to be positioned for the assault to begin
    /// </summary>
    private const float GatheredRatio = 0.8f;


    public StrategicLane SupportingLane { get; private set; }

    /// <summary>
    /// The area units gather in
    /// </summary>
    public Vector2 Gathering => SupportingLane.Position;

    /// <summary>
    /// The area units will try to take
    /// </summary>
    public Vector2 Target => SupportingLane.ForwardPosition;

    private List<CharacterAgent> _assignedAgents = [];
    public IReadOnlyList<CharacterAgent> AssignedAgents => _assignedAgents;

    private Phase _combatPhase = Phase.Gathering;


    public Offensive(StrategicLane lane)
    {
        Support(lane);
    }
    [MemberNotNull(nameof(SupportingLane))]
    public void Support(StrategicLane lane)
    {
        SupportingLane = lane;
    }

    public void Assign(CharacterAgent agent)
    {
        _assignedAgents.Add(agent);

        agent.AssignTask(GetPhaseTask());
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
            agent.AssignTask(GetPhaseTask());
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
                // If all agents are close enough, procede to next phase. Any agent not moving to location is set to move to location
                int gathered = _assignedAgents.Count(agent => agent.Character.Position.DistanceSquaredTo(Gathering) <= GatheringReadinessSquared);
                GD.Print(gathered);
                if (gathered > _assignedAgents.Count * GatheredRatio)
                {
                    StartAssault(service, chunkArray);
                }
                // units not in position to charge are fixed
                else
                {
                    foreach (CharacterAgent agent in _assignedAgents.Where(agent => agent.Task is not HoldTask task || task.TargetArea != Gathering))
                    {
                        agent.AssignTask(GetPhaseTask());
                    }
                }
                break;
            case Phase.Assaulting:

                break;
            case Phase.Consolidating:
                break;
        }
    }

    /// <summary>
    /// Gets a unit's default task for the combat phase
    /// </summary>
    /// <returns></returns>
    private AbstractAgentTask GetPhaseTask()
    {
        switch (_combatPhase)
        {
            case Phase.Gathering:
            default:
                return new HoldTask(Gathering, GatheringRadius);
            case Phase.Assaulting:
            case Phase.Consolidating:
            case Phase.Routed:
                return new HoldTask(Target, GatheringRadius);
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