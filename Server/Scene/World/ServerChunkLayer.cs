using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Resources;
using OpenTrenches.Common.World;

namespace OpenTrenches.Server.Scene.World;

public partial class ServerChunkLayer : Node2D
{
    private ChunkTileMapLayer<ServerChunk> TileMaps;

    protected IChunkArray2D<ServerChunk> Source { get; }

    public ServerChunkLayer(IChunkArray2D<ServerChunk> ChunkGrid)
    {
        Source = ChunkGrid;

        TileMaps = new(Source);
        AddChild(TileMaps);
    }
}
