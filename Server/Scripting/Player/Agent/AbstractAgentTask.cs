using System;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
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
    private IWorldObject? _currentTarget;

    public override bool Reason(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        if (TaskServices.FindTarget(character, queryService) is IWorldObject target)
            _currentTarget = target;
        
        return false;
    }

    public override void Process(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        if (TaskServices.EnemyValid(character, _currentTarget, 20)) 
        {
            character.Direction = _currentTarget.Position;
            TaskServices.ReasonAttack(character);
        }
        else
        {
            _currentTarget = null;
            character.TryClear(CharacterState.Shooting);
        }
    }
}

/// <summary>
/// Immediate objective of agent, such as getting to safety, securing an area, or holding a position.
/// Should not be used for greater focuses like defense vs offense or for very single actions like digging one tile or shooting one target.
/// </summary>
public abstract class AbstractAgentTask
{
    /// <summary>
    /// Reasons about the task to execute, return true if the task has been completed
    /// </summary>
    /// <remarks>
    /// Intended to be called infrequently
    /// </remarks>
    public abstract bool Reason(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks);

    /// <summary>
    /// Performs fast paced reactions to the task. 
    /// </summary>
    /// <remarks>
    /// Intended to be called frequently
    /// </remarks>
    public abstract void Process(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks);
}
