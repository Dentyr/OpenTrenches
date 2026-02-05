using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO;
using OpenTrenches.Common.Scene.World;
using OpenTrenches.Common.World;

namespace OpenTrenches.Server.Scene.World;

public partial class ChunkLayer : AbstractChunkLayer<BaseChunkNode>
{

    public ChunkLayer(ChunkArray2D ChunkGrid) : base(ChunkGrid) {}

    public bool Build(Vector2I cell, TileType buildTarget, float progress)
    {
        return Source.Build(cell.X, cell.Y, buildTarget, progress);
    }
}