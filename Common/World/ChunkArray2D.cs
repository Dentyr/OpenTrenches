using System;
using System.Collections.Generic;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;

namespace OpenTrenches.Common.World;

public class ChunkArray2D : IChunkArray2D
{
    private Grid2D<Chunk> Chunks { get; } = new(CommonDefines.WorldSize, CommonDefines.WorldSize, (_, _) => new Chunk());

    public int SizeX => Chunks.SizeX;
    public int SizeY => Chunks.SizeY;

    public Chunk this[int x, int y] => Chunks[x, y];

    public ChunkArray2D() {}


    public event Action<ChunkRecord>? ChunkChangedEvent;

    //* Tile changes
    //*

    /// <summary>
    /// Returns true if <paramref name="cell"/> exists, returning the tile in <paramref name="tile"/>. Tile may be null (default tile).
    /// </summary>
    public bool TryGetTile(int x, int y, out Tile? tile)
    {
        if (x >= 0 && y >= 0 && Chunks.TryGet(x / CommonDefines.ChunkSize, y / CommonDefines.ChunkSize, out Chunk? chunk)) 
        {
            tile = chunk[x % CommonDefines.ChunkSize, y % CommonDefines.ChunkSize];
            return true;
        }
        tile = default;
        return false;
    }

    /// <summary>
    /// Sets <paramref name="cell"/> to <paramref name="tile"/>, if <paramref name="cell"/> exists.
    /// </summary>
    public bool TrySetTile(int x, int y, Tile? tile)
    {
        if (Chunks.TryGet(x / CommonDefines.ChunkSize, y / CommonDefines.ChunkSize, out Chunk? chunk))
        {
            chunk[x % CommonDefines.ChunkSize, y % CommonDefines.ChunkSize] = tile;
            return true;
        }
        return false;
    }

    //* Networking changes interface
    //*

    



    /// <summary>
    /// Updates the chunk described by <paramref name="record"/>
    /// </summary>
    /// <returns>true if successful</returns>
    public bool SetChunk(ChunkRecord record)
    {
        if (Chunks.ContainsPosition(record.X, record.Y)) 
        {
            Chunks[record.X, record.Y] = record.Chunk;
            ChunkChangedEvent?.Invoke(record);
            return true;
        }
        return false;
    }
}
