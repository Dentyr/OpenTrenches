using System;

namespace OpenTrenches.Common.World;

/// <summary>
/// A chunk that contains a grid of indexable tiles
/// </summary>
public interface ITileChunk
{
    public TileType this[int x, int y] { get; }

    public event Action<TileType, int, int> TerrainChangeEvent;
}


public record ChunkRecord<TChunk>(TChunk Chunk, int X, int Y);