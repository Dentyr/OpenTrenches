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
    public readonly Character Character;
    public Team Team => Character.Team;
    public ushort CharacterId => Character.ID;

    public AbstractAgentTask Task { get; private set; }

    public CharacterAgent(Character character)
    {
        Character = character;
        Task = new IdleTask();
    }

    /// <summary>
    /// Called on each tick, thinks about what the character should do next
    /// </summary>
    public void Think(IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        Task.Process(Character, queryService, chunks);
    }

    public void Plan(IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        if (Character.Hp <= 0)
            Character.RequestRespawn();

        Task = Task.Reason(Character, queryService, chunks);
        
    }

    public void AssignTask(AbstractAgentTask task)
    {
        Task = task;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public enum AgentRole
{
    /// <summary>
    /// Sappers expanded trenches. When the trench system is complete, they become holders
    /// </summary>
    Sapper,
    /// <summary>
    /// Holders defend a position
    /// </summary>
    Holder,
    /// <summary>
    /// Assaults move around and gather for charges
    /// </summary>
    Assaulter,
}