using System;
using System.Collections.Generic;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO;

namespace OpenTrenches.Common.World;


/// <summary>
/// Chunks are a store of tiles and structures in a localized area
/// </summary>
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

    private HashSet<int> _structures = [];
    public IReadOnlySet<int> Structures => _structures;

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

    public void AddStructureId(int structureId)
    {
        _structures.Add(structureId);
    }
    public void RemoveStructureId(int structureId)
    {
        _structures.Remove(structureId);
    }
}
public interface IChunk
{
    public Tile? this[int x, int y]
    {
        get;
    }

    public IReadOnlySet<int> Structures { get; }

    public event Action<Tile?, int, int> TerrainChangeEvent;
}


public record ChunkRecord(Chunk Chunk, byte X, byte Y);