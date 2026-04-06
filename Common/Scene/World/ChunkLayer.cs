using System;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.Resources;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Scene.World;

public partial class ChunkLayer : Node2D
{
    private readonly TileMapLayer GrassLayer;
    private readonly TileMapLayer TrenchLayer;

    private readonly TileMapLayer WallLayer;

    protected ChunkArray2D Source { get; }

    public ChunkLayer(ChunkArray2D ChunkGrid)
    {
        Source = ChunkGrid;
        Source.ChunkChangedEvent += SetChunk;


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
        int cellsX = Source.SizeX * CommonDefines.ChunkSize;
        int cellsY = Source.SizeY * CommonDefines.ChunkSize;
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
        int cellsX = Source.SizeX * CommonDefines.ChunkSize;
        int cellsY = Source.SizeY * CommonDefines.ChunkSize;

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
    private void InitializeChunks(ChunkArray2D chunks)
    {
        // Load chunk states
        for (int x = 0; x < chunks.SizeX; x ++) 
        {
            for (int y = 0; y < chunks.SizeY; y ++) 
            {
                IChunk chunk = chunks[x, y];

                LoadChunk(x, y, chunk);
                chunk.TerrainChangeEvent += UpdateTile;
            }
        }
    }

    /// <summary>
    /// Updates the tiles at <paramref name="x"/> and <paramref name="y"/> to match <paramref name="tile"/>
    /// </summary>
    private void UpdateTile(Tile? tile, int x, int y)
    {
        SetTerrain([new(x, y)], tile?.Type);
    }

    private void LoadChunk(int chunkx, int chunky, IChunk chunk)
    {

        int xoffset = chunkx * CommonDefines.ChunkSize;
        int yoffset = chunky * CommonDefines.ChunkSize;

        // Batch terrain setting for efficiency
        List<Vector2I> TrenchTerrain = [];
        for (int cellx = 0; cellx < CommonDefines.ChunkSize; cellx ++)
        {
            int x = xoffset + cellx;
            for (int celly = 0; celly < CommonDefines.ChunkSize; celly ++)
            {
                int y = yoffset + celly;


                // For each cell in the chunk, add it to its respective terrain list
                if (chunk[cellx, celly]?.Type == TileType.Trench)
                {
                    TrenchTerrain.Add(new(x, y));
                }
            }
        }
        SetTerrain([..TrenchTerrain], TileType.Trench);
        // GrassLayer.SetCellsTerrainConnect([..Terrains[terrainset]], 0, terrainset);
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


    private void SetChunk(ChunkRecord record)
    {
        SetChunk(record.X, record.Y, record.Chunk);
    }
    private void SetChunk(int x, int y, Chunk chunk)
    {
        LoadChunk(x, y, chunk);
    }

    private record struct TerrainSetRecord(
        int Set,
        int Terrain
    ) {}
}
