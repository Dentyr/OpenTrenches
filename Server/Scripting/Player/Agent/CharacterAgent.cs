using System;
using Godot;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.Teams;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// makes NPC character decisions
/// </summary>
public class CharacterAgent
{
    private readonly Character _character;
    public Team Team => _character.Team;

    private AbstractAgentTask _task;

    public CharacterAgent(Character character)
    {
        _character = character;
        _task = new IdleTask();
    }

    /// <summary>
    /// Called on each tick, thinks about what the character should do next
    /// </summary>
    public void Think(IWorld2DQueryService queryService)
    {
        if (GD.Randf() > 0.975f)
        {
            _task = _task.Reason(_character, queryService);
        }

        _task.Process(_character, queryService);
    }

    public void Plan(IWorld2DQueryService adapter)
    {
        if (_character.Hp <= 0)
            _character.RequestRespawn();

    }

    public void Task(AbstractAgentTask task)
    {
        _task = task;
    }
}