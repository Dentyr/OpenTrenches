using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;

namespace OpenTrenches.Common.World;

public interface IChunkArray2D
{
    public Chunk this[int x, int y] { get; }

    public int ChunkSizeX { get; }
    public int ChunkSizeY { get; }

    public event Action<ChunkRecord>? ChunkChangedEvent;


    /// <summary>
    /// Returns true if <paramref name="cell"/> exists, returning the tile in <paramref name="tile"/>.
    /// </summary>
    public bool TryGetTile(int x, int y, [NotNullWhen(true)] out TileType? tile);
    public bool TryGetTile(Vector2I cell, [NotNullWhen(true)] out TileType? tile) => TryGetTile(cell.X, cell.Y, out tile);

    public bool TryGetCell(int x, int y, [NotNullWhen(true)] out CellRecord? cell);
    public bool TryGetCell(Vector2I position, [NotNullWhen(true)] out CellRecord? cell) => TryGetCell(position.X, position.Y, out cell);
}

public record class CellRecord(
    TileType? Tile,
    TileConstruction? TileConstruction
);