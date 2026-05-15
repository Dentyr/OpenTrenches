using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// Strategic lanes represent a horizontal slice of the map, and the strategy around fortifying the positions in that lane
/// </summary>
public class StrategicLane : AbstractObjective
{
    /// <summary>
    /// A list of positions within an area, which the AI will attempt to dig out.
    /// </summary>
    private static readonly IReadOnlyList<Vector2I> AreaEntrenchmentPattern;

    static StrategicLane()
    {
        AreaEntrenchmentPattern = [
            // * right line
            new(7, 0),
            new(7, 1),
            new(7, 2),

            new(7, 3),
            new(8, 3),
            new(9, 3),

            new(9, 4),
            new(9, 5),
            new(9, 6),
            new(9, 7),

            new(9, 8),
            new(8, 8),
            new(7, 8),

            new(7, 9),
            new(7, 10),
            new(7, 11),

            // * left line


            new(3, 0),
            new(3, 1),
            new(3, 2),

            new(3, 3),
            new(4, 3),
            new(5, 3),

            new(5, 4),
            new(5, 5),
            new(5, 6),
            new(5, 7),

            new(5, 8),
            new(4, 8),
            new(3, 8),

            new(3, 9),
            new(3, 10),
            new(3, 11),
        ];
    }

    /// <summary>
    /// If the % of sappers is less that this, will add a new defender as a sapper
    /// </summary>
    private const float SapperPercent = 0.3f;
    private const int MaxSappers = 3;


    private const float TestChance = 0.2f;

    private const float PushDistance = 15f;

    /// <summary>
    /// Cells this lane is currently trying to entrench
    /// </summary>
    private List<Vector2I> _entrenchmentPattern;

    public Vector2 Position => new((Advancement + 0.5f) * CommonDefines.AreaSize, (Lane + 0.5f) * CommonDefines.AreaSize);

    /// <summary>
    /// The height area this defensive point is in charge of 
    /// </summary>
    /// <value></value>
    public int Lane { get; private set; }
    /// <summary>
    /// How far along 
    /// </summary>
    /// <value></value>
    public int Advancement { get; private set; }

    /// <summary>
    /// Direction to advance in with testers
    /// </summary>
    private readonly int _direction;

    /// <summary>
    /// Location to send testers to to check if coast is clear
    /// </summary>
    public Vector2 ForwardPosition => Position + new Vector2(_direction * PushDistance,  0);

    /// <summary>
    /// Whether or not this position is sufficiently entrenched
    /// </summary>
    public bool Entrenched { get; private set; } = false;

    private List<DefensivePointAssignmentRecord> _assignedAgents = [];
    public IReadOnlyList<DefensivePointAssignmentRecord> AssignedAgents => _assignedAgents;


    public StrategicLane(int lane, int advancement, int direction)
    {
        Lane = lane;
        Advancement = advancement;
        
        _direction = Math.Sign(direction);
        if (_direction == 0) throw new ArgumentException("direction must be a nonzero integer");

        SetEntrenchmentPattern(new(Advancement * CommonDefines.AreaSize, Lane * CommonDefines.AreaSize));
    }

    [MemberNotNull(nameof(_entrenchmentPattern))]
    private void SetEntrenchmentPattern(Vector2I offset)
    {
        _entrenchmentPattern = [..AreaEntrenchmentPattern.Select(position => position + offset)];
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
            _entrenchmentPattern
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
