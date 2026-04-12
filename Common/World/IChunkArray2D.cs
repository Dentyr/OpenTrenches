using System;
using Godot;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;

namespace OpenTrenches.Common.World;

public interface IChunkArray2D
{
    public Chunk this[int x, int y] { get; }

    public int SizeX { get; }
    public int SizeY { get; }

    public event Action<ChunkRecord>? ChunkChangedEvent;


    /// <summary>
    /// Returns true if <paramref name="cell"/> exists, returning the tile in <paramref name="tile"/>. Tile may be null (default tile).
    /// </summary>
    public bool TryGetTile(int x, int y, out Tile? tile);
    public bool TryGetTile(Vector2I cell, out Tile? tile) => TryGetTile(cell.X, cell.Y, out tile);
}