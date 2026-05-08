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

    


    public void AddCharacter(Character character)
    {
        _agent[character.ID] = new CharacterAgent(character);
    }

    public bool RemoveCharacter(Character character)
    {
        return _agent.Remove(character.ID);
    }

    public void Process(World2DQueryService world)
    {
        foreach(var agent in _agent.Values)
        {
            agent.Think(world);
        }
    }
}