using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;

namespace OpenTrenches.Common.World;

public abstract class AbstractChunk : ITileChunk
{
    private Grid2D<TileType> Tiles { get; }
    public TileType this[int x, int y]
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

    /// <summary>
    /// Tiles being updated
    /// </summary>
    private Dictionary<Vector2I, TileConstruction> _tileConstructions = [];
    public IReadOnlyDictionary<Vector2I, TileConstruction> TileConstructions => _tileConstructions;

    public TileType[][] CopyTiles() => Tiles.CopyTiles();
    public T[][] Select<T>(Func<TileType, T> selector) => Tiles.CopySelect(selector);


    /// <summary>
    /// Called when terrin is set at (x, y)
    /// </summary>
    public event Action<TileType, int, int>? TerrainChangeEvent;

    /// <summary>
    /// Called when the tile construction at the local position (x, y) is set or removed
    /// </summary>
    public event Action<TileConstruction?, int, int>? TileConstructionSet;

    public AbstractChunk(TileType[][] tiles, IEnumerable<TileConstructionRecord> constructions)
    {
        if (tiles.Length < CommonDefines.ChunkSize || tiles.Any(arr => arr.Length < CommonDefines.ChunkSize))
            throw new Exception("Provided tile grid too small");

        Tiles = new(CommonDefines.ChunkSize, CommonDefines.ChunkSize, (x, y) => tiles[x][y]);

        // convert records to a dictionary
        _tileConstructions = new(constructions.Select(
            construct => new KeyValuePair<Vector2I, TileConstruction>(
                key: new(construct.X, construct.Y), 
                value: construct.Status
            )
        ));
    }
    public AbstractChunk(Func<int, int, TileType>? Initializer = null)
    {
        Tiles = new(CommonDefines.ChunkSize, CommonDefines.ChunkSize, Initializer);
    }
    public TileConstruction? GetTileConstructionStatus(int x, int y)
    {
        return _tileConstructions.TryGetValue(new(x, y), out var status) ? status : null;
    }

    public void SetTileConstructionStatus(int x, int y, TileConstruction construction)
    {
        _tileConstructions[new(x, y)] = construction;
        TileConstructionSet?.Invoke(construction, x, y);
    }





    public IEnumerable<TileConstructionRecord> GetActiveEarthworks()
    {
        return _tileConstructions.Select(
            kvp => new TileConstructionRecord(kvp.Key.X, kvp.Key.Y, kvp.Value)
        );
    }
}
