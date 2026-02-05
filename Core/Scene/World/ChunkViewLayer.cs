using System;
using Godot;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Scene;
using OpenTrenches.Common.Scene.World;
using OpenTrenches.Common.World;

namespace OpenTrenches.Core.Scene.World;
public partial class RenderChunkLayer : AbstractChunkLayer<BaseChunkNode>
{
    public RenderChunkLayer(ChunkArray2D ChunkGrid) : base(ChunkGrid)
    {}

    protected override BaseChunkNode _Initialize(IChunk chunk) => new BaseChunkNode(chunk);
}