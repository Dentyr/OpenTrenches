using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;

namespace OpenTrenches.Common.World;

public class ChunkArray2D : IChunkArray2D
{
    private readonly Grid2D<Chunk> Chunks = new(CommonDefines.WorldSize, CommonDefines.WorldSize, (_, _) => new Chunk());

    public int ChunkSizeX => Chunks.SizeX;
    public int ChunkSizeY => Chunks.SizeY;
    public int CellSizeX => CommonDefines.ChunkSize * ChunkSizeX;
    public int CellSizeY => CommonDefines.ChunkSize * ChunkSizeY;

    public Chunk this[int x, int y] => Chunks[x, y];

    public ChunkArray2D() {}


    public event Action<ChunkRecord>? ChunkChangedEvent;

    /// <returns>True if all points in <paramref name="area"/> are within this chunk array</returns>
    public bool IsAreaInBounds(int minx, int miny, int maxx, int maxy)
    {
        //reverse value if not by convention
        if (maxy < miny)
            (miny, maxy) = (maxy, miny);
        if (maxx < minx)
            (minx, maxx) = (maxx, minx);
        // within borderse if minimum values are greater than or equal to (0,0) and maxmimum values are within the borders
        return minx >= 0 &&
            miny >= 0 &&
            maxx <= CellSizeX &&
            maxy <= CellSizeY;
    }
    public bool IsPositionInBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && 
            x <= CellSizeX && y <= CellSizeY;
    }

    /// <summary>
    /// Tries to get the chunk that contains the cell <paramref name="x"/>, <paramref name="y"/>>
    /// </summary>
    /// <returns>True if found</returns>
    public bool TryGetChunkContaining(int x, int y, [NotNullWhen(true)] out Chunk? chunk)
    {
        return Chunks.TryGet(x / CommonDefines.ChunkSize, y / CommonDefines.ChunkSize, out chunk);
    }

    //* Tile changes
    //*

    /// <summary>
    /// Returns true if <paramref name="cell"/> exists, returning the tile in <paramref name="tile"/>. Tile may be null (default tile).
    /// </summary>
    public bool TryGetTile(int x, int y, out Tile? tile)
    {
        if (TryGetChunkContaining(x, y, out Chunk? chunk)) 
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
        if (TryGetChunkContaining(x, y, out Chunk? chunk))
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
