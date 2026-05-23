using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Resources;
using OpenTrenches.Common.Scene;
using OpenTrenches.Common.World;

namespace OpenTrenches.Core.Scene.World;


public partial class ClientChunkLayer : Node2D
{
    private ChunkTileMapLayer? TileMaps;

    private ITileArray2D? _source;

    public ClientChunkLayer()
    {
    }

    public void SetArray(ITileArray2D ChunkGrid)
    {
        TileMaps?.QueueFreeDeferred();

        _source = ChunkGrid;
        TileMaps = new(_source);
        AddChild(TileMaps);
    }
}
