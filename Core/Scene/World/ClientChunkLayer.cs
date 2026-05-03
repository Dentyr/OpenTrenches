using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Resources;
using OpenTrenches.Common.World;

namespace OpenTrenches.Core.Scene.World;


public partial class ClientChunkLayer : Node2D
{
    private ChunkTileMapLayer TileMaps;

    protected ITileArray2D Source { get; }

    public ClientChunkLayer(ITileArray2D ChunkGrid)
    {
        Source = ChunkGrid;
        
        TileMaps = new(Source);
        AddChild(TileMaps);
    }
}
