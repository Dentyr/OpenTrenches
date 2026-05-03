using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Resources;

namespace OpenTrenches.Common.World;

/// <summary>
/// Transfers the <see cref="TileType"/>s in a grid of <typeparamref name="TChunk"/>s into tilemaps that represent them
/// </summary>
public partial class ChunkTileMapLayer : Node2D
{
    private readonly TileMapLayer GrassLayer;
    private readonly TileMapLayer TrenchLayer;

    private readonly TileMapLayer WallLayer;

    protected ITileArray2D Source { get; }

    public ChunkTileMapLayer(ITileArray2D ChunkGrid)
    {
        Source = ChunkGrid;


        // set chunks 
        TrenchLayer = new()
        {
            TileSet = TileSetLibrary.TrenchTileSet,
        };
        AddChild(TrenchLayer);
        GrassLayer = new()
        {
            TileSet = TileSetLibrary.GrassTileSet,
        };
        AddChild(GrassLayer);

        WallLayer = new()
        {
            TileSet = TileSetLibrary.WallTileSet,
        };
        AddChild(WallLayer);


        // Initializing defaults
        Initializefill();
        InitializeBorder();
        InitializeTiles(Source);


        Source.TerrainChangeEvent += 
            (tile, x, y) => 
            UpdateTile(tile, new(x, y));


    }
    /// <summary>
    /// Fills the tilemaps with their default tiles
    /// </summary>
    private void Initializefill()
    {
        for (int x = 0; x < Source.SizeX; x ++)
        {
            for (int y = 0; y < Source.SizeY; y ++)
            {
                Vector2I cell = new(x, y);
                GrassLayer.SetCell(cell, TileSetDefines.DefualtTerrainAtlasID, TileSetDefines.FillAtlasPosition);
                TrenchLayer.SetCell(cell, TileSetDefines.MiscCellAltasID, TileSetDefines.MiscClearedPosition);
            }
            
        }
    }
    /// <summary>
    /// Sets the border surrounding the tilemap
    /// </summary>
    private void InitializeBorder()
    {
        int cellsX = Source.SizeX;
        int cellsY = Source.SizeY;

        //Initizing border
        List<Vector2I> border = new((cellsX + 2) * (cellsY + 2));
        for (int x = -1; x < cellsX + 1; x ++)
        {
            border.Add(new(x, -1));
            border.Add(new(x, cellsY));
        }
        for (int y = -1; y < cellsY + 1; y ++)
        {
            border.Add(new(-1, y));
            border.Add(new(cellsX, y));
        }
        WallLayer.SetCellsTerrainConnect([..border], 0, 0);
        GrassLayer.SetCellsTerrainConnect([..border], 0, 0);
        TrenchLayer.SetCellsTerrainConnect([..border], 0, 1);
    }
    /// <summary>
    /// Initializes the additional constructions from <paramref name="tiles"/>, such as trenches, and implements.
    /// </summary>
    private void InitializeTiles(ITileArray2D tiles)
    {
        // Batch terrain setting for efficiency
        Godot.Collections.Array<Vector2I> TrenchTerrain = [];
        Godot.Collections.Array<Vector2I> GrassTerrain = [];
        for (int x = 0; x < tiles.SizeX; x ++)
        {
            for (int y = 0; y < tiles.SizeY; y ++)
            {
                // For each cell in the chunk, add it to its respective terrain list
                switch (tiles[x, y])
                {
                    case TileType.Clear:
                        GrassTerrain.Add(new(x, y));
                        break;
                    case TileType.Trench:
                        TrenchTerrain.Add(new(x, y));
                        break;
                }

            }
        }
        SetTerrain([..TrenchTerrain], TileType.Trench);
        SetTerrain([..GrassTerrain], TileType.Clear);

    }

    /// <summary>
    /// Updates the tiles at <paramref name="x"/> and <paramref name="y"/> to match <paramref name="tile"/>
    /// </summary>
    private void UpdateTile(TileType tile, Vector2I position)
    {
        SetTerrain([position], tile);
    }

    private void SetTerrain(Godot.Collections.Array<Vector2I> points, TileType? type)
    {
        switch (type)
        {
            case TileType.Trench:
                TrenchLayer.SetCellsTerrainConnect(points, 0, 0);
                GrassLayer.SetCellsTerrainConnect(points, 0, -1);
                break;
            case TileType.Clear:
            case null:
                TrenchLayer.SetCellsTerrainConnect(points, 0, 1);
                GrassLayer.SetCellsTerrainConnect(points, 0, 0);
                break;
        }
    }
}
