using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Resources;

namespace OpenTrenches.Common.World;

/// <summary>
/// Transfers the <see cref="TileType"/>s in a grid of <typeparamref name="TChunk"/>s into tilemaps that represent them
/// </summary>
public partial class ChunkTileMapLayer<TChunk> : Node2D where TChunk : ITileChunk
{
    private readonly TileMapLayer GrassLayer;
    private readonly TileMapLayer TrenchLayer;

    private readonly TileMapLayer WallLayer;

    protected IChunkArray2D<TChunk> Source { get; }

    public ChunkTileMapLayer(IChunkArray2D<TChunk> ChunkGrid)
    {
        Source = ChunkGrid;
        Source.ChunkChangedEvent += SetChunkTiles;


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
        InitializeChunks(Source);



    }
    /// <summary>
    /// Fills the tilemaps with their default tiles
    /// </summary>
    private void Initializefill()
    {
        int cellsX = Source.ChunkSizeX * CommonDefines.ChunkSize;
        int cellsY = Source.ChunkSizeY * CommonDefines.ChunkSize;
        for (int x = 0; x < cellsX; x ++)
        {
            for (int y = 0; y < cellsY; y ++)
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
        int cellsX = Source.ChunkSizeX * CommonDefines.ChunkSize;
        int cellsY = Source.ChunkSizeY * CommonDefines.ChunkSize;

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
    /// Initializes the additional constructions from <paramref name="chunks"/>, such as trenches, and implements.
    /// </summary>
    private void InitializeChunks(IChunkArray2D<TChunk> chunks)
    {
        // Load chunk states
        for (int x = 0; x < chunks.ChunkSizeX; x ++) 
        {
            for (int y = 0; y < chunks.ChunkSizeY; y ++) 
            {
                TChunk chunk = chunks[x, y];

                LoadChunk(x, y, chunk);
            }
        }
    }

    /// <summary>
    /// Updates the tiles at <paramref name="x"/> and <paramref name="y"/> to match <paramref name="tile"/>
    /// </summary>
    private void UpdateTile(TileType tile, Vector2I position)
    {
        SetTerrain([position], tile);
    }

    private void LoadChunk(int chunkx, int chunky, TChunk chunk)
    {

        int xoffset = chunkx * CommonDefines.ChunkSize;
        int yoffset = chunky * CommonDefines.ChunkSize;

        // Batch terrain setting for efficiency
        List<Vector2I> TrenchTerrain = [];
        List<Vector2I> GrassTerrain = [];
        for (int cellx = 0; cellx < CommonDefines.ChunkSize; cellx ++)
        {
            int x = xoffset + cellx;
            for (int celly = 0; celly < CommonDefines.ChunkSize; celly ++)
            {
                int y = yoffset + celly;


                // For each cell in the chunk, add it to its respective terrain list
                switch (chunk[cellx, celly])
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

        // Watch updates
        chunk.TerrainChangeEvent += 
            (tile, cellX, cellY) => 
            UpdateTile(
                tile, 
                new(
                    cellX + xoffset, 
                    cellY + yoffset)
            );

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


    private void SetChunkTiles(ChunkRecord<TChunk> record)
    {
        LoadChunk(record.X, record.Y, record.Chunk);
    }
}
