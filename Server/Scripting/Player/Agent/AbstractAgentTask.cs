using System;
using Godot;
using OpenTrenches.Common.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// represent the overall task the agent is trying to accomplish, such as a holding a position, or taking a position
/// </summary>
public enum AgentStance
{
    Defensive,
    Aggressive,
    Reckless
}

public class IdleTask : AbstractAgentTask
{
    public override AbstractAgentTask Reason(Character character, ICharacterAdapter adapter)
    {
        
        if (TaskServices.FindTarget(character, adapter) is IWorldObject target)
            return new HoldTask(target);
        
        return this;
    }

    public override void Process(Character character, ICharacterAdapter adapter)
    {
        
    }
}

/// <summary>
/// Immediate objective of agent, such as getting to safety, securing an area, or holding a position.
/// Should not be used for greater focuses like defense vs offense or for very single actions like digging one tile or shooting one target.
/// </summary>
public abstract class AbstractAgentTask
{
    /// <summary>
    /// Reasons about the task to execute, returning itself if incomplete or a new task if task is changed.
    /// Intended to be called infrequently
    /// </summary>
    public abstract AbstractAgentTask Reason(Character character, ICharacterAdapter adapter);

    /// <summary>
    /// Performs fast paced reactions to the task. 
    /// Intended to be called frequently
    /// </summary>
    public abstract void Process(Character character, ICharacterAdapter adapter);
}
