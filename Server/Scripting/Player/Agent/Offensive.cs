using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Teams;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;


public class Offensive : AbstractObjective
{
    private const float GatheringRadius = 6f;
    private const float GatheringReadinesError = 3f;
    private const float GatheringReadinesDistance = GatheringRadius + GatheringReadinesError;

    /// <summary>
    /// % of troops needed to be positioned for the assault to begin
    /// </summary>
    private const float GatheredRatio = 0.8f;

    private readonly Team _team;

    public StrategicLane SupportingLane { get; private set; }

    /// <summary>
    /// How far along the lane the gathering position is
    /// </summary>
    public int GatheringForward { get; private set; }

    /// <summary>
    /// How far along the lane the target position is
    /// </summary>
    public int TargetForward { get; private set; }


    private List<CharacterAgent> _assignedAgents = [];
    public IReadOnlyList<CharacterAgent> AssignedAgents => _assignedAgents;

    private Phase _combatPhase = Phase.Gathering;


    public Offensive(Team team, StrategicLane lane)
    {
        _team = team;
        Support(lane);
    }
    [MemberNotNull(nameof(SupportingLane))]
    public void Support(StrategicLane lane)
    {
        SupportingLane = lane;
        _combatPhase = Phase.Gathering;

        GatheringForward = SupportingLane.Forward;
        TargetForward = SupportingLane.Forward + 1;

        foreach (CharacterAgent agent in _assignedAgents)
            agent.AssignTask(GetPhaseTask());
    }


    /// <summary>
    /// The area units gather in
    /// </summary>
    private Vector2I GetGatheringArea() => AreaTranslationService.GetAreaFromForward(SupportingLane.Direction, SupportingLane.Lane, GatheringForward);
    private Vector2 GetGatheringPoint() => AreaTranslationService.GetAreaCenter(GetGatheringArea());

    /// <summary>
    /// The area units will try to take
    /// </summary>
    private Vector2I GetTargetingArea() => AreaTranslationService.GetAreaFromForward(SupportingLane.Direction, SupportingLane.Lane, TargetForward);
    private Vector2 GetTargetingPoint() => AreaTranslationService.GetAreaCenter(GetTargetingArea());

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

    public override void Strategize(IWorld2DQueryService service, IServerChunkArray chunkArray)
    {
        switch (_combatPhase)
        {
            case Phase.Gathering:
                // If all agents are close enough, procede to next phase. Any agent not moving to location is set to move to location
                int gathered = _assignedAgents.Count(agent => agent.Character.Position.ChebyshevDistanceTo(GetGatheringPoint()) <= GatheringReadinesDistance);
                if (gathered > _assignedAgents.Count * GatheredRatio)
                {
                    StartAssault(service, chunkArray);
                }
                // units not in position to charge are fixed
                else
                {
                    foreach (CharacterAgent agent in _assignedAgents.Where(agent => agent.Task is not HoldTask task || task.TargetArea != GetGatheringPoint()))
                    {
                        agent.AssignTask(GetPhaseTask());
                    }
                }
                break;
            case Phase.Assaulting:

                break;
        }
    }

    /// <summary>
    /// Returns true if the gathering point is not friendly or contested
    /// </summary>
    public bool IsGatheringPointLost(IWorld2DQueryService service, IServerChunkArray chunkArray)
    {
        Occupation gatheringOccupation = WorldAreaService.CheckOccupation(GetGatheringArea(), _team, service, chunkArray);
        if (gatheringOccupation == Occupation.Hostile || gatheringOccupation == Occupation.Neutral) return true;
        return false;
    }
    /// <summary>
    /// Returns true if the target area is controlled by friendly units
    /// </summary>
    public bool IsTargetSecured(IWorld2DQueryService service, IServerChunkArray chunkArray)
    {
        Occupation targetOccupation = WorldAreaService.CheckOccupation(GetTargetingArea(), _team, service, chunkArray);
        if (targetOccupation == Occupation.Friendly) return true;
        return false;
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
                return new HoldTask(GetGatheringPoint(), GatheringRadius);
            case Phase.Assaulting:
                return new HoldTask(GetTargetingPoint(), GatheringRadius);
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
    }
}