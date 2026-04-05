using System;
using System.Collections.Generic;
using Godot;
using OpenTrenches.Common.Collections;
using OpenTrenches.Common.Contracts.Defines;
using OpenTrenches.Common.Contracts.DTO.UpdateModel;
using OpenTrenches.Common.World;

namespace OpenTrenches.Common.Scene.World;

public partial class ChunkLayer : Node2D
{
    private readonly TileMapLayer TerrainLayer;
    private readonly TileMapLayer WallLayer;

    protected ChunkArray2D Source { get; }

    public ChunkLayer(ChunkArray2D ChunkGrid)
    {
        Source = ChunkGrid;
        Source.ChunkChangedEvent += SetChunk;


        // set chunks 
        TerrainLayer = new()
        {
            TileSet = TileSetLibrary.GrassTileSet,
        };
        AddChild(TerrainLayer);

        WallLayer = new()
        {
            TileSet = TileSetLibrary.WallTileSet,
        };
        AddChild(WallLayer);

        // Load each chunk into tilemaplayer
        for (int x = 0; x < ChunkGrid.SizeX; x ++) 
        {
            for (int y = 0; y < ChunkGrid.SizeY; y ++) 
            {
                Initialize(x, y, ChunkGrid[x, y]);
            }
        }
    }

    private void Initialize(int x, int y, IChunk chunk)
    {
        LoadChunk(x, y, chunk);
        chunk.TerrainChangeEvent += UpdateTile;
    }

    private void UpdateTile(Tile? tile, int x, int y)
    {
        TerrainLayer.SetCellsTerrainConnect([new(x, y)], GetTerrainFromTile(tile?.Type), 0);
    }

    private void LoadChunk(int x, int y, IChunk chunk)
    {
        int xoffset = x * CommonDefines.ChunkSize;
        int yoffset = y * CommonDefines.ChunkSize;

        // Batch terrain setting for efficiency
        List<Vector2I>[] Terrains = [[], []];
        for (int cellx = 0; cellx < CommonDefines.ChunkSize; cellx ++)
        {
            for (int celly = 0; celly < CommonDefines.ChunkSize; celly ++)
            {
                // For each cell in the chunk, add it to its respective terrain list
                Vector2I position = new(xoffset + cellx, yoffset + celly);
                int terrainSet = GetTerrainFromTile(chunk[x, y]?.Type);
                Terrains[terrainSet].Add(position);
            }
        }
        // for each terrain list, set cells to respective terrain type.
        for (int terrainset = 0; terrainset < Terrains.Length; terrainset ++)
        {
            if (Terrains[terrainset].Count < 1) continue; // skip if empty
            TerrainLayer.SetCellsTerrainConnect([..Terrains[terrainset]], terrainset, 0);
        }
    }
    /// <summary>
    /// Returns the tile map layer index associated with the tile type.
    /// A number between 0 and 1 inclusive
    /// </summary>
    private int GetTerrainFromTile(TileType? type)
    {
        return type switch
        {
            TileType.Trench => 0,
            _ => 0,
        };
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