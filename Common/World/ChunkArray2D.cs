using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;

namespace OpenTrenches.Common.World;

public abstract class ChunkArray2D<TChunk> where TChunk : ITileChunk
{
    private readonly Grid2D<TChunk> Chunks;

    public int ChunkSizeX => Chunks.SizeX;
    public int ChunkSizeY => Chunks.SizeY;
    public int CellSizeX => CommonDefines.ChunkSize * ChunkSizeX;
    public int CellSizeY => CommonDefines.ChunkSize * ChunkSizeY;

    public event Action<ChunkRecord<TChunk>>? ChunkChangedEvent;

    public TChunk this[int x, int y]
    {
        get => Chunks[x, y];
        set
        {
            Chunks[x, y] = value;
            ChunkChangedEvent?.Invoke(new(value, x, y));
        }
    }

    public ChunkArray2D(Func<int, int, TChunk>? Initializer = null)
    {
        Chunks = new(CommonDefines.WorldSize, CommonDefines.WorldSize, Initializer);
    }


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
            x < CellSizeX && y < CellSizeY;
    }
    public bool IsChunkInBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && 
            x < ChunkSizeX && y < ChunkSizeY;
    }

    /// <summary>
    /// Tries to get the chunk that contains the cell <paramref name="x"/>, <paramref name="y"/>>
    /// </summary>
    /// <returns>True if found</returns>
    public bool TryGetChunkContaining(int x, int y, [NotNullWhen(true)] out TChunk? chunk)
    {
        if (x < 0 || y < 0)
        {
            chunk = default;
            return false;
        }
        return Chunks.TryGet(x / CommonDefines.ChunkSize, y / CommonDefines.ChunkSize, out chunk);
    }


    //* Tile changes
    //*

    /// <summary>
    /// Returns true if <paramref name="cell"/> exists, returning the tile in <paramref name="tile"/>.
    /// </summary>
    public bool TryGetTile(int x, int y, [NotNullWhen(true)] out TileType? tile)
    {
        if (TryGetChunkContaining(x, y, out TChunk? chunk)) 
        {
            int chunkx = x % CommonDefines.ChunkSize;
            int chunky = y % CommonDefines.ChunkSize;
            tile = chunk[chunkx, chunky];
            return true;
        }
        tile = null!;
        return false;
    }

    /// <summary>
    /// Sets <paramref name="cell"/> to <paramref name="tile"/>, if <paramref name="cell"/> exists.
    /// </summary>
    public bool TrySetTile(int x, int y, TileType tile)
    {
        if (TryGetChunkContaining(x, y, out TChunk? chunk))
        {
            chunk[x % CommonDefines.ChunkSize, y % CommonDefines.ChunkSize] = tile;
            _OnTileSet(x, y, tile);
            return true;
        }
        return false;
    }
    protected virtual void _OnTileSet(int x, int y, TileType tile) {}




    //* Networking changes interface
    //*

    



}
