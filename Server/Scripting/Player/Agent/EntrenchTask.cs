using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;
using OpenTrenches.Core.Scripting.World;
using OpenTrenches.Server.Scripting.World;

namespace OpenTrenches.Server.Scripting.Player.Agent;

/// <summary>
/// Shoots at the closest enemy character.
/// </summary>
public class EntrenchTask : AbstractAgentTask
{
    private List<Vector2I> _positions;

    // utility
    private Vector2I _next;

    public EntrenchTask(IEnumerable<Vector2I> positions)
    {
        _positions = [..positions];
        if (_positions.Count > 0)
            _next = _positions[^1];
    }

    public override AbstractAgentTask Reason(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        if (_positions.Count == 0) return new HoldTask();

        _next = _positions[^1];
        while (chunks[_next.X, _next.Y] == TileType.Trench)
        {
            _positions.RemoveAt(_positions.Count - 1);
            if (_positions.Count == 0) return new HoldTask();
            _next = _positions[^1];
        }
        if (TaskServices.Navigate(character, _next.CellToPosition(), queryService))
        {
            character.SetBuildTarget(_next.X, _next.Y, TileType.Trench);
        }
        return this;
    }

    public override bool Process(Character character, IWorld2DQueryService queryService, IServerChunkArray chunks)
    {
        TaskServices.Step(character, _next.CellToPosition(), chunks, error: 0.3f);

        if (_positions.Count == 0) 
            return true;
        return false;
    }
}
