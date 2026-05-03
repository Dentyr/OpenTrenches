using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;

namespace OpenTrenches.Common.World;

public abstract class ChunkArray2D
{
    private Grid2D<TileType> Tiles { get; }
    public TileType this[int x, int y]
    {
        get => Tiles[x, y];
        set 
        {
            if (x < 0 || y < 0 || x > SizeX || y > SizeY) 
                throw new IndexOutOfRangeException();

            if (Tiles[x, y] != value)
            {
                Tiles[x, y] = value;
                TerrainChangeEvent?.Invoke(value, x, y);
            }
        }
    }

    public int SizeX => CommonDefines.WorldSize;
    public int SizeY => CommonDefines.WorldSize;


    public TileType[][] CopyTiles() => Tiles.CopyTiles();
    public T[][] Select<T>(Func<TileType, T> selector) => Tiles.CopySelect(selector);

    /// <summary>
    /// Called when terrin is set at (x, y)
    /// </summary>
    public event Action<TileType, int, int>? TerrainChangeEvent;


    //* original

    public ChunkArray2D()
    {
        Tiles = new(CommonDefines.WorldSize, CommonDefines.WorldSize, (_, _) => TileType.Clear);
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
            maxx <= SizeX &&
            maxy <= SizeY;
    }
    public bool IsPositionInBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && 
            x < SizeX && y < SizeY;
    }


    //* Tile changes
    //*

    /// <summary>
    /// Returns true if <paramref name="cell"/> exists, returning the tile in <paramref name="tile"/>.
    /// </summary>
    public bool TryGetTile(int x, int y, [NotNullWhen(true)] out TileType? tile)
    {
        if (IsPositionInBounds(x, y))
        {
            tile = this[x, y];
            return true;
        }
        tile = null;
        return false;
    }

    /// <summary>
    /// Sets <paramref name="cell"/> to <paramref name="tile"/>, if <paramref name="cell"/> exists.
    /// </summary>
    public bool TrySetTile(int x, int y, TileType tile)
    {
        if (IsPositionInBounds(x, y))
        {
            this[x, y] = tile;
            _OnTileSet(x, y, tile);
            return true;
        }
        return false;
    }
    protected virtual void _OnTileSet(int x, int y, TileType tile) {}




    //* Networking changes interface
    //*

    



}
