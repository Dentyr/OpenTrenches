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

    private bool _entrech;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position">Where to go</param>
    /// <param name="entrench">If it should look for a trench to jump into</param>
    public PositionTask(Vector2 position, bool entrench)
    {
        _position = position;
        _entrech = entrench;
    }

    public override bool Process(Character character, IWorld2DQueryService queryService)
    {
        if (character.Position.DistanceSquaredTo(_position) < 60f)
        {
            return true;
        }
        return false;
    }

    public override AbstractAgentTask Reason(Character character, IWorld2DQueryService queryService)
    {
        if (character.Position.DistanceSquaredTo(_position) > 100f)
        {
            //TODO pathfind
            // check for trench
            // adapter.Query()
            // if trench exists, then check occupancy, and then jump in 
            // if full, extend
            // if no trench make new trench
            character.MoveIn(_position - character.Position);
            return this;
        }
        // once reached destination, hold
        character.MoveIn(Vector2.Zero);
        return new HoldTask();
    }
}