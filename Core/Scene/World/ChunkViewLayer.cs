using System;
using Godot;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Scene;
using OpenTrenches.Common.Scene.World;
using OpenTrenches.Common.World;

namespace OpenTrenches.Core.Scene.World;
public partial class ChunkViewLayer : AbstractChunkLayer<BaseChunkNode>
{
    public ChunkViewLayer(ChunkArray2D ChunkGrid) : base(ChunkGrid)
    {}
}