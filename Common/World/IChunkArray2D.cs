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
    /// Increases building progress for the cell at (<paramref name="x"/>, <paramref name="y"/>). Returns true if completed building or cannot build, and false if it is still progressing build. 
    /// </summary>
    public bool ProgressBuild(int x, int y, float progress);
    public bool ProgressBuild(Vector2I buildCell, float progress) => ProgressBuild(buildCell.X, buildCell.Y, progress);

    public void StartBuild(int x, int y, TileType buildTarget, float initialProgress);
    public void StartBuild(Vector2I buildCell, TileType buildTarget, float initialProgress = 0) => StartBuild(buildCell.X, buildCell.Y, buildTarget, initialProgress);

    /// <summary>
    /// Returns true if <paramref name="cell"/> exists, returning the tile in <paramref name="tile"/>. Tile may be null (default tile).
    /// </summary>
    public bool TryGetTile(int x, int y, out Tile? tile);
    public bool TryGetTile(Vector2I cell, out Tile? tile) => TryGetTile(cell.X, cell.Y, out tile);
}