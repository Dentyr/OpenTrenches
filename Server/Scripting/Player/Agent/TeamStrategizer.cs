using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Server.Scene.World;
using OpenTrenches.Server.Scripting.Teams;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// Manages a team's strategic AI decisions
/// </summary>
public class TeamStrategizer
{
    private const int PointCount = 12;
    public readonly Team Team;

    private DefensivePoint[] _defensePoints;

    private readonly Dictionary<ushort, CharacterAgent> _agent = [];

    /// <summary>
    /// List of agents assigned to this strategizer
    /// </summary>
    private List<CharacterAgent> _agentList = [];
    private int _strategyCounter = 0;

    public TeamStrategizer(Team team)
    {
        Team = team;
        // makes a defensive line with 5 points along the map
        _defensePoints = new DefensivePoint[PointCount];
        for (int i = 0; i < PointCount; i ++)
        {
            //? Currently this only works for a two team setup. More complexity for multi teams will be required\
            float offset = team.ID % 2 == 0 ? -30 : 30;
            _defensePoints[i] = new()
            {
                Position = new(
                    CommonDefines.WorldSize / 2 + offset, 
                    CommonDefines.WorldSize * ((i + 0.5f) / (PointCount))
                )
            };
        }
    }

    public void AddCharacter(Character character)
    {
        CharacterAgent agent = new(character);
        if (_agent.TryAdd(character.ID, agent))
        {
            _agentList.Add(agent);

            // Assign to least defended point
            var point = _defensePoints.MinBy(pt => pt.AssignedDefenders.Count);
            point ??= _defensePoints[0];

            point.AssignedDefenders.Add(agent);
            agent.Task(new PositionTask(point.Position, true));
        }
    }

    public bool RemoveCharacter(Character character)
    {
        if (_agent.Remove(character.ID, out var item))
        {
            _agentList.Remove(item);
            return true;
        }
        return false;
    }

    public void Organize(IWorld2DQueryService queryService)
    {
        
    }

    internal void Calculate(IWorld2DQueryService world)
    {
        foreach(var agent in _agent.Values)
        {
            agent.Think(world);
        }


        if (_agentList.Count > 0)
        {
            _strategyCounter ++;
            if (_strategyCounter >= _agentList.Count) _strategyCounter = 0;
            _agentList[_strategyCounter].Plan(world);
        }
    }
}

public class DefensivePoint
{
    public Vector2 Position;
    public List<CharacterAgent> AssignedDefenders = [];

}