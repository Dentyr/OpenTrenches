using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO;

namespace OpenTrenches.Common.World;


public class Chunk
{
    private Grid2D<Tile?> Tiles { get; }
    public Tile? this[int x, int y]
    {
        get => Tiles[x, y];
    }
    public void SetTerrain(Tile terrain, byte x, byte y)
    {
        if (x >= CommonDefines.ChunkSize || y >= CommonDefines.ChunkSize) return;

        if (Tiles[x, y] != terrain)
        {
            Tiles[x, y] = terrain;
            TerrainChangeEvent?.Invoke(terrain, x, y);
        }
    }

    public Tile?[][] CopyTiles() => Tiles.CopyTiles();
    public T[][] Select<T>(Func<Tile?, T> selector) => Tiles.CopySelect(selector);


    /// <summary>
    /// Called when terrin is set at (x, y)
    /// </summary>
    public event Action<Tile, byte, byte>? TerrainChangeEvent;
    public Chunk()
    {
        Tiles = new(CommonDefines.ChunkSize, CommonDefines.ChunkSize);

        //TODO testing
        int half = CommonDefines.ChunkSize / 2;
        for (int i = 0; i < CommonDefines.ChunkSize; i ++)
        {
            Tiles[half, i] = new(TileType.Trench, 100);
            Tiles[half + 3, i] = new(TileType.Trench, 100);
            Tiles[half + 2, i] = new(TileType.Trench, 100);
            Tiles[half + 1, i] = new(TileType.Trench, 100);
            Tiles[half - 1, i] = new(TileType.Trench, 100);
            Tiles[half - 2, i] = new(TileType.Trench, 100);
            Tiles[half - 3, i] = new(TileType.Trench, 100);

            Tiles[half, i] = new(TileType.Trench, 100);
            Tiles[i, half + 3] = new(TileType.Trench, 100);
            Tiles[i, half + 2] = new(TileType.Trench, 100);
            Tiles[i, half + 1] = new(TileType.Trench, 100);
            Tiles[i, half - 1] = new(TileType.Trench, 100);
            Tiles[i, half - 2] = new(TileType.Trench, 100);
            Tiles[i, half - 3] = new(TileType.Trench, 100);
        }
    }


}


public record ChunkRecord(Chunk Chunk, byte X, byte Y);