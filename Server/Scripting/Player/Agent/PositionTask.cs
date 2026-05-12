using Godot;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// The characer attempts to get to a position while shooting at threats
/// </summary>
public class PositionTask : AbstractAgentTask
{
    /// <summary>
    /// Moves to this position and seeks a good place to entrench, unless significantly threatened
    /// </summary>
    private Vector2 _position;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="position">Where to go</param>
    public PositionTask(Vector2 position)
    {
        _position = position;
    }

    public override bool Process(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        if (TaskServices.Step(character, _position, chunks, 
            error: 1)
        ) {
            return true;
        }
        return false;
    }

    public override AbstractAgentTask Reason(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        if (TaskServices.Navigate(
            character, _position, queryService, 
            error: 10
        )) return new HoldTask();
        return this;
    }
}