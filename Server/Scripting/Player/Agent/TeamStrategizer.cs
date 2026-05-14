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
    /// <summary>
    /// Desired defensive points
    /// </summary>
    private const int PointCount = 12;
    /// <summary>
    /// Desired defender count per defensive point
    /// </summary>
    private const int DesiredDefenders = 5;

    private const int MaxOffensiveTroops = 25;


    //* Calculation frequency settings

    /// <summary>
    /// How many ticks should occur before a defense point should strategize
    /// </summary>
    private const int StrategizationIntermittency = 10;

    private const int AgentsThinkingPerTick = 40;
    private const int AgentsReasoningPerTick = 5;

    //* Strategy configuration

    private const int TargetAssaultLength = 30;



    //* Team information
    public readonly Team Team;

    /// <summary>
    /// The general direction that units should move towards to advance. Intended for a 2 team setup
    /// </summary>
    private readonly Vector2I _direction;


    //* strategy

    private readonly DefensivePoint[] _defensePoints = new DefensivePoint[PointCount];
    private int _objectiveStrategizationCounter = 0;

    private readonly List<Offensive> _offensives = [];

    //* agents

    private readonly Dictionary<ushort, CharacterAgent> _agent = [];


    /// <summary>
    /// List of agents assigned to this strategizer
    /// </summary>
    private List<CharacterAgent> _agentList = [];
    private int _thinkCounter = 0;
    private int _reasonCounter = 0;



    public TeamStrategizer(Team team)
    {
        Team = team;

        /// <summary>
        /// From the center of the field, where the initial trench lines are
        /// </summary>
        float initialDefensivePointOffset;
        //TODO think of a better way to differentiate teams
        // Currently only works for a two team setup
        // right team
        if (team.ID % 2 == 0)
        {
            _direction = Vector2I.Right;
            initialDefensivePointOffset = -30;
        }
        else
        {
            _direction = Vector2I.Left;
            initialDefensivePointOffset = 30;
        }

        // makes a defensive line along the map
        for (int i = 0; i < _defensePoints.Length; i ++)
        {
            _defensePoints[i] = new()
            {
                Position = new(
                    CommonDefines.WorldSize / 2 + initialDefensivePointOffset, 
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

            // If defensive points already have enough defenders, place them into an offensive group
            if (point.AssignedAgents.Count > DesiredDefenders)
            {
                Offensive? offensive;

                // Assign to the offensive with least attackers, creating a new one if there is no offensive or if too many are assigned
                offensive = _offensives.MinBy(offensive => offensive.AssignedAgents.Count);
                if (offensive is null || offensive.AssignedAgents.Count > MaxOffensiveTroops)
                    offensive = NewOffensive();

                offensive.Assign(agent);
            }
            else
            {
                point.Assign(agent);
            }
        }
    }
    /// <summary>
    /// Creates a new offensive with no units assigned
    /// </summary>
    private Offensive NewOffensive()
    {

        Offensive offensive = new(_defensePoints[0].Position, _defensePoints[0].Position + (_direction * TargetAssaultLength));
        _offensives.Add(offensive);
        return offensive;
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

    private IEnumerable<AbstractObjective> GetAllObejctives()
    {
        return _defensePoints.Concat<AbstractObjective>(_offensives);
    }

    public void Calculate(IWorld2DQueryService world, IServerChunkArray chunkArray)
    {

        // If there are any agents, reason a few of them each tick
        if (_agentList.Count > 0)
        {
            int numThinking = Math.Min(AgentsThinkingPerTick, _agentList.Count);
            for (int i = 0; i < numThinking; i ++)
            {
                _thinkCounter = (_thinkCounter + 1) % _agentList.Count;
                _agentList[_thinkCounter].Think(world, chunkArray);
            }

            int numReasoning = Math.Min(AgentsReasoningPerTick, _agentList.Count);
            for (int i = 0; i < numReasoning; i ++)
            {
                _reasonCounter = (_reasonCounter + 1) % _agentList.Count;
                _agentList[_reasonCounter].Plan(world, chunkArray);
            }
        }
        // If there are any objectives, strategize one of them every few ticks
        if (GetAllObejctives().Any())
        {
            IEnumerable<AbstractObjective> objectives = GetAllObejctives();
            int numObjectives = objectives.Count();
            _objectiveStrategizationCounter = (_objectiveStrategizationCounter + 1) % (numObjectives * StrategizationIntermittency);
            if (_objectiveStrategizationCounter % StrategizationIntermittency == 0)
                objectives.ElementAt(_objectiveStrategizationCounter / StrategizationIntermittency).Strategize(world, chunkArray);
        }
    }
}

