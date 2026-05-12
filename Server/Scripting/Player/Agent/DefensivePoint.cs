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
public class DefensivePoint
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

    public Vector2 Position;
    /// <summary>
    /// Whether or not this position is sufficiently entrenched
    /// </summary>
    /// <value></value>
    public bool Entrenched { get; private set; } = false;

    private List<AgentAssignmentRecord> _assignedAgents = [];
    public IReadOnlyList<AgentAssignmentRecord> AssignedAgents => _assignedAgents;

    public void Assign(CharacterAgent agent)
    {
        AgentRole wanted = AgentRole.Holder;
        // if not entrenched and wanting more sappers, add as a sapper
        if (!Entrenched && 
            (
                _assignedAgents.Count(assigned => assigned.Role == AgentRole.Sapper)
                < Math.Min(_assignedAgents.Count, MaxSappers)
            )
        ) {
            wanted = AgentRole.Sapper;
        }

        _assignedAgents.Add(new(agent, wanted));
    }
    public void Unassign(CharacterAgent agent)
    {
        _assignedAgents.RemoveAll(record => record.Agent == agent);
    }

    public void Strategize(IWorld2DQueryService service, IServerChunkArray chunkArray)
    {
        // Get all the agents doing trench digging
        IEnumerable<CharacterAgent> _freeSappers = AssignedAgents
            .Where(record => 
                record.Role == AgentRole.Sapper && 
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
            GD.Print($"tryna sap {_plannedDigging.Length}");
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
            if (record.Role == AgentRole.Holder)
            {
                if (record.Agent.Character.Position.DistanceTo(Position) > 15)
                    record.Agent.AssignTask(new PositionTask(Position));
            }
        }

    }
}
