using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;

namespace OpenTrenches.Common.World;

public interface ITileArray2D
{
    public TileType this[int x, int y] { get; }

    public int SizeX { get; }
    public int SizeY { get; }
    
    public event Action<TileType, int, int>? TerrainChangeEvent;
}

public record class CellRecord(
    TileType? Tile,
    TileConstruction? TileConstruction
);