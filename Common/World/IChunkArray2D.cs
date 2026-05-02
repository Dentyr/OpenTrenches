using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;

namespace OpenTrenches.Common.World;

public interface IChunkArray2D<TChunk>
{
    public TChunk this[int x, int y] { get; }

    public int ChunkSizeX { get; }
    public int ChunkSizeY { get; }

    public event Action<ChunkRecord<TChunk>>? ChunkChangedEvent;
}

public record class CellRecord(
    TileType? Tile,
    TileConstruction? TileConstruction
);