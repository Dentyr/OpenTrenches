using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.World;

namespace OpenTrenches.Core.Scene.World;

/// <summary>
/// Chunks are a store of tiles and structures in a localized area
/// </summary>
public class ClientChunk : AbstractChunk
{
    public ClientChunk(TileType[][] tiles, IEnumerable<TileConstructionRecord> constructions)
        : base(tiles, constructions)
    {}
    public ClientChunk()
    {}
}
