using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.NativeInterop;
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
                    CommonDefines.WorldSize * ((i + 0.5f) / PointCount)
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
            var point = _defensePoints.MinBy(pt => pt.AssignedAgents.Count);
            point ??= _defensePoints[0];

            point.Assign(agent);
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

    public void Calculate(IWorld2DQueryService world, IServerChunkArray chunkArray)
    {
        foreach(var agent in _agent.Values)
        {
            agent.Think(world, chunkArray);
        }


        if (_agentList.Count > 0)
        {
            for (int i = 0; i < 10; i ++)
            {
                _strategyCounter = (_strategyCounter + 1) % _agentList.Count;
                _agentList[_strategyCounter].Plan(world, chunkArray);
            }
        }
        if (_defensePoints.Length > 0 && GD.Randf() > 0.9f)
        {
            _defensePoints[Random.Shared.Next(_defensePoints.Length)].Strategize(world, chunkArray);
        }
    }
}
