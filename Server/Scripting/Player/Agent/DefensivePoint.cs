using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// Defensive points represent a position and the strategy around fortifying the position
/// </summary>
public class DefensivePoint : AbstractObjective
{
    /// <summary>
    /// A list of positions relative to the defensive point, which the AI will attempt to dig out.
    /// </summary>
    private static readonly IReadOnlyList<Vector2I> _entrenchPosition;

    static DefensivePoint()
    {
        _entrenchPosition = [
            // normal line
            new (0, -4),
            new (0, -3),
            new (0, -2),
            new (0, -1),
            new (0, 0),
            new (0, 1),
            new (0, 2),
            new (0, 3),
            new (0, 4),

            // connecting paths
            new (-1, 3),
            new (-2, 3),
            new (-3, 3),
            new (-4, 3),

            new (-1, -3),
            new (-2, -3),
            new (-3, -3),
            new (-4, -3),


            // rear line
            new (-5, -4),
            new (-5, -3),
            new (-5, -2),
            new (-5, -1),
            new (-5, 0),
            new (-5, 1),
            new (-5, 2),
            new (-5, 3),
            new (-5, 4),
        ];
    }

    /// <summary>
    /// If the % of sappers is less that this, will add a new defender as a sapper
    /// </summary>
    private const float SapperPercent = 0.3f;
    private const int MaxSappers = 3;


    private const float TestChance = 0.2f;

    private const float PushDistance = 15f;

    private readonly Vector2 _initialPosition;
    public Vector2 Position { get; private set; }

    /// <summary>
    /// Direction to advance in with testers
    /// </summary>
    private readonly Vector2 _direction;

    /// <summary>
    /// Location to send testers to to check if coast is clear
    /// </summary>
    public Vector2 ForwardPosition => Position + _direction * PushDistance;

    /// <summary>
    /// Whether or not this position is sufficiently entrenched
    /// </summary>
    /// <value></value>
    public bool Entrenched { get; private set; } = false;

    private List<DefensivePointAssignmentRecord> _assignedAgents = [];
    public IReadOnlyList<DefensivePointAssignmentRecord> AssignedAgents => _assignedAgents;


    public DefensivePoint(Vector2 position, Vector2 direction)
    {
        _initialPosition = position;
        Position = position;

        _direction = direction.Normalized();
    }


    public void Assign(CharacterAgent agent)
    {
        DefensivePointAgentRole wanted;
        // if not entrenched and wanting more sappers, add as a sapper
        int numSappers = _assignedAgents.Count(assigned => assigned.Role == DefensivePointAgentRole.Sapper);
        int wantedSappers = Math.Min((int)Math.Ceiling((double)_assignedAgents.Count * SapperPercent), MaxSappers);

        // if there is already a tester, add as sapper if wanted or add as holder if not
        if (_assignedAgents.Any(agent => agent.Role == DefensivePointAgentRole.Tester))
        {
            if (!Entrenched && numSappers < wantedSappers) {
                wanted = DefensivePointAgentRole.Sapper;
            }
            else wanted = DefensivePointAgentRole.Holder;
        }
        else wanted = DefensivePointAgentRole.Tester; 

        _assignedAgents.Add(new(agent, wanted));
    }
    public void Unassign(CharacterAgent agent)
    {
        _assignedAgents.RemoveAll(record => record.Agent == agent);
    }

    public override void Strategize(IWorld2DQueryService service, IServerChunkArray chunkArray)
    {
        // Get all the agents doing trench digging
        IEnumerable<CharacterAgent> _freeSappers = AssignedAgents
            .Where(record => 
                record.Role == DefensivePointAgentRole.Sapper && 
                record.Agent.Task is not EntrenchTask)
            .Select(record => record.Agent);
        


        // Make them dig out the remaining of the defensive pattern
        Vector2I cellPosition = (Vector2I)Position;
        Vector2I[] _plannedDigging = [.. 
            _entrenchPosition.Select(position => position + cellPosition)
            .Where(position => chunkArray[position.X, position.Y] != TileType.Trench)
        ];
        if (_plannedDigging.Length > 0)
        {
            // start of next build range
            int buildStartIndex = 0;
            foreach(CharacterAgent sapper in _freeSappers)
            {
                // end of next build range, either 3 next or until last
                int buildEndIndex = Math.Min(buildStartIndex + 3, _plannedDigging.Length);
                Vector2I[] agentTasked = _plannedDigging[buildStartIndex .. buildEndIndex];
                sapper.AssignTask(new EntrenchTask(agentTasked));

                buildStartIndex = buildEndIndex;
            }
        }

        // gathers far away units
        foreach (var record in _assignedAgents)
        {
            if (record.Role == DefensivePointAgentRole.Holder)
            {
                if (record.Agent.Character.Position.DistanceTo(Position) > 15)
                    record.Agent.AssignTask(new HoldTask(Position));
            }
        }

        //test enemy
        if (TestChance >= GD.Randf())
        {
            IEnumerable<CharacterAgent> _testers = AssignedAgents
                .Where(record => record.Role == DefensivePointAgentRole.Tester)
                .Select(record => record.Agent);
            
            foreach (CharacterAgent tester in _testers)
            {
                tester.AssignTask(new HoldTask(ForwardPosition, 1f));
            }
        }

    }
}
