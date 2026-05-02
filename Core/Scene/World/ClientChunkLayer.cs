using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Resources;
using OpenTrenches.Common.World;

namespace OpenTrenches.Core.Scene.World;


public partial class ClientChunkLayer : Node2D
{
    private ChunkTileMapLayer<ClientChunk> TileMaps;

    protected IChunkArray2D<ClientChunk> Source { get; }

    public ClientChunkLayer(IChunkArray2D<ClientChunk> ChunkGrid)
    {
        Source = ChunkGrid;
        
        TileMaps = new(Source);
        AddChild(TileMaps);
    }
}
