using System;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Server.Scene.World;
using OpenTrenches.Server.Scripting.Teams;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// Agent Manager organizes all the AI components of two teams
/// </summary>
public class AgentManager
{
    public TeamStrategizer _team1;
    public TeamStrategizer _team2;

    
    public AgentManager(params Team[] teams)
    {
        _team1 = new(teams[0]);
        _team2 = new(teams[1]);
    }

    public void AddCharacter(Character character)
    {
        if      (character.Team == _team1.Team) _team1.AddCharacter(character);
        else if (character.Team == _team2.Team) _team2.AddCharacter(character);
    }

    public bool RemoveCharacter(Character character)
    {
        if      (character.Team == _team1.Team) return _team1.RemoveCharacter(character);
        else if (character.Team == _team2.Team) return _team2.RemoveCharacter(character);
        return false;
    }

    public void Process(IWorld2DQueryService world)
    {
        _team1.Calculate(world);
        _team2.Calculate(world);
    }
}