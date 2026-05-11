using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// Shoots at the closest enemy character.
/// </summary>
public class EntrenchTask : AbstractAgentTask
{
    private List<Vector2I> _positions;

    public EntrenchTask(IEnumerable<Vector2I> positions)
    {
        _positions = [..positions];
    }

    public override AbstractAgentTask Reason(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        if (_positions.Count == 0) 
            return new HoldTask();
        return this;
    }

    public override bool Process(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        if (_positions.Count == 0) return true;

        Vector2I next = _positions[^1];
        while (chunks[next.X, next.Y] == TileType.Trench)
        {
            _positions.RemoveAt(_positions.Count - 1);
            if (_positions.Count == 0) return true;
            next = _positions[^1];
        }
        if (TaskServices.Navigate(character, next, queryService))
        {
            character.SetBuildTarget(next.X, next.Y, TileType.Trench);
        }
        return false;
    }
}
