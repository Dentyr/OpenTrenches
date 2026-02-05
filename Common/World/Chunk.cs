using System;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO;

namespace OpenTrenches.Common.World;


public class Chunk : IChunk
{
    private Grid2D<Tile?> Tiles { get; }
    public Tile? this[int x, int y]
    {
        get => Tiles[x, y];
        set 
        {
            if (x >= CommonDefines.ChunkSize || y >= CommonDefines.ChunkSize) throw new IndexOutOfRangeException();

            if (Tiles[x, y] != value)
            {
                Tiles[x, y] = value;
                TerrainChangeEvent?.Invoke(value, x, y);
            }
        }
    }

    public Tile?[][] CopyTiles() => Tiles.CopyTiles();
    public T[][] Select<T>(Func<Tile?, T> selector) => Tiles.CopySelect(selector);


    /// <summary>
    /// Called when terrin is set at (x, y)
    /// </summary>
    public event Action<Tile?, int, int>? TerrainChangeEvent;

    public Chunk(Tile[][] tiles)
    {
        Tiles = new(CommonDefines.ChunkSize, CommonDefines.ChunkSize);
    }
    public Chunk(Func<int, int, Tile?> Initializer)
    {
        Tiles = new(CommonDefines.ChunkSize, CommonDefines.ChunkSize, Initializer);
    }
    public Chunk()
    {
        Tiles = new(CommonDefines.ChunkSize, CommonDefines.ChunkSize);
    }
}
public interface IChunk
{
    public Tile? this[int x, int y]
    {
        get;
    }

    public event Action<Tile?, int, int> TerrainChangeEvent;
}


public record ChunkRecord(Chunk Chunk, byte X, byte Y);