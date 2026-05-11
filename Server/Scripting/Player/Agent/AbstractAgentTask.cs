using System;
using Godot;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.World;

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
    public override AbstractAgentTask Reason(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        if (TaskServices.FindTarget(character, queryService) is IWorldObject target)
            return new HoldTask(target);
        
        return this;
    }

    public override bool Process(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        return false;
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
    public abstract AbstractAgentTask Reason(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks);

    /// <summary>
    /// Performs fast paced reactions to the task. 
    /// Intended to be called frequently
    /// </summary>
    /// <returns>True if it should re-reason</returns>
    public abstract bool Process(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks);
}
