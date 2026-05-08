using System;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Server.Scene.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// Agent Manager controls a number of AIs 
/// </summary>
public class AgentManager
{
    private readonly Dictionary<ushort, CharacterAgent> _agent = [];
    private List<CharacterAgent> _agentList = [];
    private int _strategyCounter = 0;

    


    public void AddCharacter(Character character)
    {
        CharacterAgent agent = new(character);
        if (_agent.TryAdd(character.ID, agent))
        {
            _agentList.Add(agent);
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

    public void Process(World2DQueryService world)
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